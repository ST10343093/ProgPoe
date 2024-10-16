using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgPoe.Data;

namespace ProgPoe.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public LecturerController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Claims(DateTime? start, DateTime? end)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = await _userManager.GetUserIdAsync(currentUser);

            var query = _dbContext.Claims
                .Include(c => c.Documents)
                .Where(c => c.ApplicationUserId == currentUserId);

            if (start.HasValue && end.HasValue)
            {
                query = query.Where(c => c.DateSubmitted >= start.Value && c.DateSubmitted <= end.Value);
            }

            var claims = await query.ToListAsync();

            return View(claims);
        }
    }
}
