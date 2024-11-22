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

            var totalClaims = claims.Count();
            var pendingClaims = claims.Count(c => c.Status == "Pending");
            var approvedClaims = claims.Count(c => c.Status == "Approved by Manager");
            var rejectedClaims = claims.Count(c => c.Status == "Rejected by Coordinator" || c.Status == "Rejected by Manager");
            var longestHoursWorked = claims.Max(c => c.HoursWorked);
            var highestHourlyRate = claims.Max(c => c.HourlyRate);
            var totalPayments = claims.Sum(c => c.TotalAmount);
            var totalpendingPayments = claims.Where(c => c.PaymentStatus == "Processing").Sum(c => c.TotalAmount);
            var totalcompletedPayments = claims.Where(c => c.PaymentStatus == "Paid").Sum(c => c.TotalAmount);

            var processingPayments = claims.Count(c => c.PaymentStatus == "Processing");
            var completedPayments = claims.Count(c => c.PaymentStatus == "Paid");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports");
            Directory.CreateDirectory(folderPath);

            var fileName = $"{reportName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(folderPath, fileName);
            var relativeFilePath = Path.Combine("reports", fileName);

            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document();
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                document.Add(new Paragraph("========================================================"));
                document.Add(new Paragraph(" My Claim App System Report"));
                document.Add(new Paragraph("========================================================"));
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("DETAILS OF REPORT"));
                document.Add(new Paragraph("--------------------------"));
                document.Add(new Paragraph($"Report: {reportName}"));
                document.Add(new Paragraph($"Type: {reportType}"));
                document.Add(new Paragraph($"Period: {startDate.ToShortDateString()} - {endDate.ToShortDateString()}"));
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("STATISTICS OF CLAIMS"));
                document.Add(new Paragraph("------------------"));

                if (reportType == "payments")
                {
                    document.Add(new Paragraph($"Total Payments: {totalPayments:C}"));
                    document.Add(new Paragraph($"Total Pending Payments: {totalpendingPayments:C}"));
                    document.Add(new Paragraph($"Total Completed Payments: {totalcompletedPayments:C}"));
                    document.Add(new Paragraph($"Processing Payments: {processingPayments}"));
                    document.Add(new Paragraph($"Completed Payments: {completedPayments}"));
                }
                else
                {
                    document.Add(new Paragraph($"Total Claims: {totalClaims}"));
                    document.Add(new Paragraph($"Pending Claims: {pendingClaims}"));
                    document.Add(new Paragraph($"Approved Claims: {approvedClaims}"));
                    document.Add(new Paragraph($"Rejected Claims: {rejectedClaims}"));
                }
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("CLAIM DATABASE"));
                document.Add(new Paragraph("------------------"));
                document.Add(new Paragraph("\n"));

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
                    table.AddCell("Hours Worked");
                    table.AddCell("Hourly Rate");
                    table.AddCell("Status");

                    foreach (var claim in claims)
                    {
                        table.AddCell(claim.ClaimId.ToString());
                        table.AddCell(claim.HoursWorked.ToString());
                        table.AddCell(claim.HourlyRate.ToString("C"));
                        table.AddCell(claim.Status);
                    }
                }

                document.Add(table);
                document.Close();

                System.IO.File.WriteAllBytes(filePath, ms.ToArray());
            }

            var report = new Report
            {
                ReportName = reportName,
                ReportType = reportType,
                StartDate = startDate,
                EndDate = endDate,
                FilePath = relativeFilePath
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            if (!ModelState.IsValid)
            {
                var existingReports = await _context.Reports.ToListAsync();
                var model = new GenerateReportViewModel
                {
                    ExistingReports = existingReports
                };
                return View(model);
            }

            return RedirectToAction(nameof(GenerateReport));
        }





    }
}

