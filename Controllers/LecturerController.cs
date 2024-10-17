using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgPoe.Data;

namespace ProgPoe.Controllers
{
    // This controller is for Lecturers, who can view their claims
    [Authorize(Roles = "Lecturer")] // Restrict access to users with the Lecturer role
    public class LecturerController : Controller
    {
        // Declare the database context and user manager for accessing claims and user info
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor to inject the necessary dependencies
        public LecturerController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext; // Initialize the database context
            _userManager = userManager; // Initialize the user manager
        }

        // GET: Display claims submitted by the current lecturer with optional date filtering
        public async Task<IActionResult> Claims(DateTime? start, DateTime? end)
        {
            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = await _userManager.GetUserIdAsync(currentUser); // Get the user's ID

            // Build a query to get claims for the current user, including supporting documents
            var query = _dbContext.Claims
                .Include(c => c.Documents) // Include related documents for each claim
                .Where(c => c.ApplicationUserId == currentUserId); // Filter by the current user's ID

            // If start and end dates are provided, filter claims by submission date
            if (start.HasValue && end.HasValue)
            {
                query = query.Where(c => c.DateSubmitted >= start.Value && c.DateSubmitted <= end.Value);
            }

            // Execute the query and retrieve the list of claims
            var claims = await query.ToListAsync();

            // Return the claims to the view for display
            return View(claims);
        }
    }
}
