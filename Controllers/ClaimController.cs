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
        // Declare dependencies for the database context, user manager, and web environment
        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<IdentityUser> _accountManager;
        private readonly IWebHostEnvironment _webEnvironment;

        // Constructor to initialize the dependencies
        public ClaimController(ApplicationDbContext databaseContext, UserManager<IdentityUser> accountManager, IWebHostEnvironment webEnvironment)
        {
            _databaseContext = databaseContext;
            _accountManager = accountManager;
            _webEnvironment = webEnvironment;
        }

        // GET: Display the claim creation form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Handle the claim creation request
        [HttpPost]
        [ValidateAntiForgeryToken] // Ensure the form is submitted securely
        public async Task<IActionResult> Create(ClaimViewModel viewModel)
        {
            // Check if the form data is valid
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Check if at least one supporting document is uploaded
            if (viewModel.SupportingDocuments == null || viewModel.SupportingDocuments.Count == 0)
            {
                ModelState.AddModelError("", "Please upload minimum one document.");
                return View(viewModel);
            }

            // Validate file types and size
            bool invalidFileDetected = false;
            foreach (var file in viewModel.SupportingDocuments)
            {
                if (file.ContentType != "application/pdf" || file.Length > 20 * 1024 * 1024)
                {
                    ViewBag.FileError = true;
                    invalidFileDetected = true;
                    ModelState.AddModelError("", "Only PDF files supported with the max size of 20 MB.");
                    return View(viewModel);
                }
            }

            // If no invalid files are detected, proceed with saving the claim
            if (!invalidFileDetected)
            {
                // Get the logged-in user
                var loggedInUser = await _accountManager.GetUserAsync(User);

                // Create a new claim with the form data
                var newClaim = new Claim
                {
                    HoursWorked = viewModel.HoursWorked,
                    HourlyRate = viewModel.HourlyRate,
                    Notes = viewModel.Notes,
                    DateSubmitted = DateTime.Now,
                    ApplicationUserId = loggedInUser.Id, // Associate claim with user
                    TotalAmount = viewModel.HourlyRate * viewModel.HoursWorked
                };

                // Add the claim to the database and save changes
                _databaseContext.Claims.Add(newClaim);
                await _databaseContext.SaveChangesAsync();

                // Set the directory path to upload supporting documents
                var uploadDirectory = Path.Combine(_webEnvironment.WebRootPath, "SupportingDocuments");

                // Save each uploaded file to the server
                foreach (var file in viewModel.SupportingDocuments)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadDirectory, uniqueFileName);

                    // Ensure the directory exists
                    Directory.CreateDirectory(uploadDirectory);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Create a record for the document in the database
                    var documentRecord = new Document
                    {
                        ClaimId = newClaim.ClaimId,
                        DocumentName = uniqueFileName,
                        FilePath = filePath
                    };

                    // Add the document record to the databas
                    _databaseContext.Documents.Add(documentRecord);
                }

                // Save all changes related to the claim and document
                await _databaseContext.SaveChangesAsync();

                // Display a success message after submitting the claim
                TempData["SuccessMessage"] = "Claim submitted successfully!";

                // Redirect to the list of claims for the lecturer
                return RedirectToAction("Claims", "Lecturer");
            }

            // If any validation fails, return the form view with errors
            return View(viewModel);
        }

        // Action to allow users to view the status of their claims
        public async Task<IActionResult> ViewClaimStatus()
        {
            // Get the currently logged-in user
            var currentUser = await _accountManager.GetUserAsync(User);

            // Retrieve the claims submitted by the current user
            var userClaims = _databaseContext.Claims
                .Where(c => c.ApplicationUserId == currentUser.Id)
                .ToList();

            // Return the claims to the view for display
            return View(userClaims);
        }

        public bool IsValidDocument(IFormFile file)

        {

            // Confirm the file is not null

            if (file == null)

            {

                return false;

            }

            // Check if the file type is PDF and size is 15 MB or less

            return file.ContentType == "application/pdf" && file.Length <= 15 * 1024 * 1024; // 15 MB

        }

    }
}
