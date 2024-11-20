using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgPoe.Models;
using ProgPoe.Data;

namespace ProgPoe.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var totalClaims = _context.Claims.Count();
            var pendingClaims = _context.Claims.Count(c => c.Status == "Pending");
            var approvedClaims = _context.Claims.Count(c => c.Status == "Approved by Manager");

            var totalPayments = _context.Claims.Where(c => c.PaymentStatus == "Paid" || c.PaymentStatus == "Processing")
                                               .Sum(c => c.TotalAmount);
            var pendingPayments = _context.Claims.Where(c => c.PaymentStatus == "Processing")
                                                 .Sum(c => c.TotalAmount);
            var completedPayments = _context.Claims.Where(c => c.PaymentStatus == "Paid")
                                                   .Sum(c => c.TotalAmount);

            var dashboardData = new DashboardViewModel
            {
                TotalClaims = totalClaims,
                PendingClaims = pendingClaims,
                ApprovedClaims = approvedClaims,
                TotalPayments = totalPayments,
                PendingPayments = pendingPayments,
                CompletedPayments = completedPayments
            };

            return View(dashboardData);
        }

        public async Task<IActionResult> ProcessPayments()
        {
            var claims = await _context.Claims
                .Include(c => c.ApplicationUser)
                .Where(c => c.Status == "Approved by Manager" && c.PaymentStatus == "Processing")
                .ToListAsync();

            var claimsprocesssed = await _context.Claims
                .Include(c => c.ApplicationUser)
                .Where(c => c.Status == "Approved by Manager" && c.PaymentStatus == "Paid")
                .ToListAsync();

            var totalClaims = claimsprocesssed.Count(c => c.PaymentStatus == "Paid");
            var totalAmountToPay = claims.Sum(c => c.TotalAmount);
            var pendingPayments = claims.Count(c => c.PaymentStatus == "Processing");

            var model = new ProcessPaymentsViewModel
            {
                Claims = claims,
                TotalClaims = totalClaims,
                TotalAmountToPay = totalAmountToPay,
                PendingPayments = pendingPayments
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int claimId)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
            {
                return NotFound();
            }

            claim.PaymentStatus = "Paid";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ProcessPayments));
        }

        public async Task<IActionResult> GenerateReport()
        {
            var existingReports = await _context.Reports.ToListAsync();

            var viewModel = new GenerateReportViewModel
            {
                ExistingReports = existingReports
            };

            return View(viewModel);
        }

        [HttpPost]

        public async Task<IActionResult> GenerateReport(string reportType, DateTime startDate, DateTime endDate, string reportName)

        {

            if (startDate > endDate)

            {

                ModelState.AddModelError(string.Empty, "Start date cannot be after the end date.");

                return View();

            }

            IQueryable<ProgPoe.Models.Claim> claimsQuery = _context.Claims.Where(c => c.DateSubmitted >= startDate && c.DateSubmitted <= endDate);

            if (reportType == "payments")

            {

                claimsQuery = claimsQuery.Where(c => c.PaymentStatus == "Processing" || c.PaymentStatus == "Paid");

            }

            var claims = await claimsQuery.ToListAsync();

            if (!claims.Any())

            {

                ModelState.AddModelError(string.Empty, "No claims found for the given date range.");

                ViewBag.NoClaimsForDateRange = true;

                return View(new GenerateReportViewModel { ExistingReports = await _context.Reports.ToListAsync() });

            }

            // Generate statistics for the report

            var totalClaims = claims.Count();

            var pendingClaims = claims.Count(c => c.Status == "Pending");

            var approvedClaims = claims.Count(c => c.Status == "Approved by Manager");

            var rejectedClaims = claims.Count(c => c.Status == "Rejected by Coordinator" || c.Status == "Rejected by Manager");

            var totalPayments = claims.Sum(c => c.TotalAmount);

            var totalPendingPayments = claims.Where(c => c.PaymentStatus == "Processing").Sum(c => c.TotalAmount);

            var totalCompletedPayments = claims.Where(c => c.PaymentStatus == "Paid").Sum(c => c.TotalAmount);

            // Create directory to save the report

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports");

            Directory.CreateDirectory(folderPath); // Ensure the directory exists

            // Generate filename

            var fileName = $"{reportName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            var filePath = Path.Combine(folderPath, fileName);

            var relativeFilePath = Path.Combine("reports", fileName);

            // Create PDF document

            using (var ms = new MemoryStream())

            {

                var document = new iTextSharp.text.Document();

                var writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                document.Add(new Paragraph("MY CLAIM APP REPORTS GENERATION"));

                document.Add(new Paragraph("========================================================"));

                document.Add(new Paragraph("\n"));

                document.Add(new Paragraph("DETAILS OF REPORT"));

                document.Add(new Paragraph("--------------------------"));

                document.Add(new Paragraph($"Report Name: {reportName}"));

                document.Add(new Paragraph($"Report Type: {reportType}"));

                document.Add(new Paragraph($"Date Range: {startDate.ToShortDateString()} - {endDate.ToShortDateString()}"));

                document.Add(new Paragraph("\n"));

                document.Add(new Paragraph("STATISTICS ON CLAIMS"));

                document.Add(new Paragraph("------------------"));

                if (reportType == "payments")

                {

                    document.Add(new Paragraph($"Total Payments: {totalPayments:C}"));

                    document.Add(new Paragraph($"Total Pending Payments: {totalPendingPayments:C}"));

                    document.Add(new Paragraph($"Total Completed Payments: {totalCompletedPayments:C}"));

                }

                else

                {

                    document.Add(new Paragraph($"Total Claims: {totalClaims}"));

                    document.Add(new Paragraph($"Pending Claims: {pendingClaims}"));

                    document.Add(new Paragraph($"Approved Claims: {approvedClaims}"));

                    document.Add(new Paragraph($"Rejected Claims: {rejectedClaims}"));

                }

                document.Add(new Paragraph("\n"));

                document.Add(new Paragraph("CLAIM LIST"));

                document.Add(new Paragraph("------------------"));

                document.Add(new Paragraph("\n"));

                // Create table based on the report type

                var table = new PdfPTable(reportType == "payments" ? 5 : 4);

                if (reportType == "payments")

                {

                    table.AddCell("Claim ID");

                    table.AddCell("Total Amount");

                    table.AddCell("Start Date");

                    table.AddCell("End Date");

                    table.AddCell("Payment Status");

                    foreach (var claim in claims)

                    {

                        table.AddCell(claim.ClaimId.ToString());

                        table.AddCell(claim.TotalAmount.ToString("C"));

                        table.AddCell(claim.StartDate.ToShortDateString());

                        table.AddCell(claim.EndDate.ToShortDateString());

                        table.AddCell(claim.PaymentStatus);

                    }

                }

                else

                {

                    table.AddCell("Claim ID");

                    table.AddCell("Claim Status");

                    table.AddCell("Total Amount");

                    table.AddCell("Payment Status");

                    foreach (var claim in claims)

                    {

                        table.AddCell(claim.ClaimId.ToString());

                        table.AddCell(claim.Status);

                        table.AddCell(claim.TotalAmount.ToString("C"));

                        table.AddCell(claim.PaymentStatus);

                    }

                }

                document.Add(table);

                document.Close();

                writer.Close();

                // Write to the file system

                using (var fileStream = new FileStream(filePath, FileMode.Create))

                {

                    ms.WriteTo(fileStream);

                }

            }

            // Redirect to the report page or show the generated report link

            return RedirectToAction(nameof(GenerateReport), new { filePath = relativeFilePath });

        }


    }
}

