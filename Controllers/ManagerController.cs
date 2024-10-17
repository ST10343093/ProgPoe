using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgPoe.Data;

namespace ProgPoe.Controllers
{
    // This controller is restricted to users with the "Manager" role
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        // Declare a database context for accessing the claims
        private readonly ApplicationDbContext _context;

        // Constructor that initializes the database context
        public ManagerController(ApplicationDbContext context)
        {
            _context = context; // Inject the ApplicationDbContext
        }

        // GET: Displays all claims that have been approved by the Coordinator but not yet approved by the Manager
        public IActionResult Index()
        {
            // Query claims that are approved by the Coordinator and pending Manager approval
            var pendingClaims = _context.Claims
                .Include(c => c.ApplicationUser) // Include related ApplicationUser data
                .Include(c => c.Documents) // Include related Documents data
                .Where(c => c.IsApprovedByCoordinator && !c.IsApprovedByManager && c.Status == "Approved by Coordinator") // Filter the claims
                .ToList(); // Convert the result to a list

            // Pass the list of pending claims to the view for display
            return View(pendingClaims);
        }

        // POST: Approves a claim by the Manager
        [HttpPost]
        public async Task<IActionResult> Approve(int claimId)
        {
            // Find the claim by its ID
            var claim = await _context.Claims.FindAsync(claimId);

            // If the claim is found, update its approval status and mark it as approved by the Manager
            if (claim != null)
            {
                claim.IsApprovedByManager = true; // Set the Manager's approval flag to true
                claim.Status = "Approved by Manager"; // Update the claim status
                await _context.SaveChangesAsync(); // Save changes to the database
            }

            // Redirect back to the index view after approval
            return RedirectToAction("Index");
        }

        // POST: Rejects a claim by the Manager
        [HttpPost]
        public async Task<IActionResult> Reject(int claimId)
        {
            // Find the claim by its ID
            var claim = await _context.Claims.FindAsync(claimId);

            // If the claim is found, update its status to rejected by the Manager
            if (claim != null)
            {
                claim.Status = "Rejected by Manager"; // Update the claim status to rejected
                await _context.SaveChangesAsync(); // Save changes to the database
            }

            // Redirect back to the index view after rejection
            return RedirectToAction("Index");
        }
    }
}
