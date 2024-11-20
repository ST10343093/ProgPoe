using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgPoe.Data;
using ProgPoe.Models;

namespace ProgPoe.Controllers
{
    // This controller manages claims and requires the user to be a Lecturer
    [Authorize(Roles = "Lecturer")]
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ClaimController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if ((model.EndDate - model.StartDate).Days > 31 || model.StartDate.Month != model.EndDate.Month)
            {
                ModelState.AddModelError("", "The date range must be within one month and cannot exceed 31 days.");
                return View(model);
            }

            var currentDate = DateTime.Now;
            var validMonths = new[] { currentDate.Month, currentDate.AddMonths(-1).Month };
            if (!validMonths.Contains(model.StartDate.Month) || !validMonths.Contains(model.EndDate.Month))
            {
                ModelState.AddModelError("", "Claims can only be submitted for the current or previous month.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            bool existingClaim = _context.Claims.Any(c =>
                c.ApplicationUserId == user.Id &&
                c.StartDate.Month == model.StartDate.Month &&
                c.StartDate.Year == model.StartDate.Year &&
                (c.Status == "Pending" || c.Status == "Approved by Manager" || c.Status == "Approved by Coordinator")
            );

            if (existingClaim)
            {
                ModelState.AddModelError("", "You have already submitted a claim for this month, and it is either pending or approved.");
                ViewData["ClaimExists"] = existingClaim;
                return View(model);
            }

            if (model.SupportingDocuments == null || model.SupportingDocuments.Count == 0)
            {
                ModelState.AddModelError("", "At least one supporting document must be attached.");
                return View(model);
            }

            foreach (var file in model.SupportingDocuments)
            {
                if (!IsValidDocument(file))
                {
                    ModelState.AddModelError("", "Only PDF files under 15 MB are allowed.");
                    return View(model);
                }
            }

            var claim = new Claim
            {
                HoursWorked = model.HoursWorked,
                HourlyRate = model.HourlyRate,
                Notes = model.Notes,
                DateSubmitted = DateTime.Now,
                ApplicationUserId = user.Id,
                TotalAmount = model.HourlyRate * model.HoursWorked,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "SupportingDocuments");
            foreach (var file in model.SupportingDocuments)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                Directory.CreateDirectory(uploadsFolder);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var document = new Document
                {
                    ClaimId = claim.ClaimId,
                    DocumentName = uniqueFileName,
                    FilePath = filePath
                };

                _context.Documents.Add(document);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim submitted successfully!";
            return RedirectToAction("Claims", "Lecturer");
        }

        public async Task<IActionResult> TrackClaims()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var claims = _context.Claims
                .Where(c => c.ApplicationUserId == currentUser.Id)
                .ToList();

            return View(claims);
        }

        public bool IsValidDocument(IFormFile file)
        {
            if (file == null)
            {
                return false;
            }

            return file.ContentType == "application/pdf" && file.Length <= 15 * 1024 * 1024;
        }
    }
}
