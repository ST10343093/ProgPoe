using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgPoe.Data;

namespace ProgPoe.Controllers
{
    // This controller is for the Coordinator to manage and approve/reject claims
    public class CoordinatorController : Controller
    {
        // Declare the database context for accessing claims
        private readonly ApplicationDbContext _dbContext;

        // Constructor to inject the database context
        public CoordinatorController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Display a list of pending claims for the Coordinator
        public IActionResult Index()
        {
            // Fetch all claims that are pending approval by the Coordinator
            var pendingClaims = _dbContext.Claims
                .Include(c => c.ApplicationUser) // Include the user details
                .Include(c => c.Documents) // Include the associated documents
                .Where(c => !c.IsApprovedByCoordinator && c.Status == "Pending")
                .ToList(); // Convert the result to a list

            // Pass the list of pending claims to the view for display
            return View(pendingClaims);
        }

        // POST: Approve a specific claim by its ID
        [HttpPost]
        public async Task<IActionResult> Approve(int claimId)
        {
            // Find the claim in the database using its ID
            var claim = await _dbContext.Claims.FindAsync(claimId);

            // If the claim exists, update its status and mark it as approved
            if (claim != null)
            {
                claim.IsApprovedByCoordinator = true; // Set the approval flag
                claim.Status = "Approved by Coordinator"; // Update the status
                await _dbContext.SaveChangesAsync(); // Save changes to the database
            }

            // Redirect back to the Index action to see the updated list of claims
            return RedirectToAction("Index");
        }

        // POST: Reject a specific claim by its ID
        [HttpPost]
        public async Task<IActionResult> Reject(int claimId)
        {
            // Find the claim in the database using its ID
            var claim = await _dbContext.Claims.FindAsync(claimId);

            // If the claim exists, update its status to "Rejected"
            if (claim != null)
            {
                claim.Status = "Rejected by Coordinator"; // Update the status
                await _dbContext.SaveChangesAsync(); // Save changes to the database
            }

            // Redirect back to the Index action to see the updated list of claims
            return RedirectToAction("Index");
        }
    }
}
