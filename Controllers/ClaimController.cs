using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgPoe.Data;
using ProgPoe.Models;

namespace ProgPoe.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<IdentityUser> _accountManager;
        private readonly IWebHostEnvironment _webEnvironment;

        public ClaimController(ApplicationDbContext databaseContext, UserManager<IdentityUser> accountManager, IWebHostEnvironment webEnvironment)
        {
            _databaseContext = databaseContext;
            _accountManager = accountManager;
            _webEnvironment = webEnvironment;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.SupportingDocuments == null || viewModel.SupportingDocuments.Count == 0)
            {
                ModelState.AddModelError("", "At least one supporting document must be attached.");
                return View(viewModel);
            }

            bool invalidFileDetected = false;
            foreach (var file in viewModel.SupportingDocuments)
            {
                if (file.ContentType != "application/pdf" || file.Length > 15 * 1024 * 1024)
                {
                    ViewBag.FileError = true;
                    invalidFileDetected = true;
                    ModelState.AddModelError("", "Only PDF files under 15 MB are allowed.");
                    return View(viewModel);
                }
            }

            if (!invalidFileDetected)
            {
                var loggedInUser = await _accountManager.GetUserAsync(User);

                var newClaim = new Claim
                {
                    HoursWorked = viewModel.HoursWorked,
                    HourlyRate = viewModel.HourlyRate,
                    Notes = viewModel.Notes,
                    DateSubmitted = DateTime.Now,
                    ApplicationUserId = loggedInUser.Id,
                    TotalAmount = viewModel.HourlyRate * viewModel.HoursWorked
                };

                _databaseContext.Claims.Add(newClaim);
                await _databaseContext.SaveChangesAsync();

                var uploadDirectory = Path.Combine(_webEnvironment.WebRootPath, "documents");

                foreach (var file in viewModel.SupportingDocuments)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadDirectory, uniqueFileName);

                    Directory.CreateDirectory(uploadDirectory);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var documentRecord = new Document
                    {
                        ClaimId = newClaim.ClaimId,
                        DocumentName = uniqueFileName,
                        FilePath = filePath
                    };

                    _databaseContext.Documents.Add(documentRecord);
                }

                await _databaseContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Claims", "Lecturer");
            }
            return View(viewModel);
        }

        public async Task<IActionResult> ViewClaimStatus()
        {
            var currentUser = await _accountManager.GetUserAsync(User);
            var userClaims = _databaseContext.Claims
                .Where(c => c.ApplicationUserId == currentUser.Id)
                .ToList();
            return View(userClaims);
        }
    }
}
