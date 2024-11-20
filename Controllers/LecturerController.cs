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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LecturerController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Claims(DateTime? startDate, DateTime? endDate)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = await _userManager.GetUserIdAsync(user);

            var claimsQuery = _context.Claims
                .Include(c => c.Documents)
                .Where(c => c.ApplicationUserId == userId);

            if (startDate.HasValue && endDate.HasValue)
            {
                claimsQuery = claimsQuery.Where(c => c.DateSubmitted >= startDate.Value && c.DateSubmitted <= endDate.Value);
            }

            var claims = await claimsQuery.ToListAsync();

            return View(claims);
        }
    }
}

