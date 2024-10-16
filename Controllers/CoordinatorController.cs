using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgPoe.Data;

namespace ProgPoe.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CoordinatorController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var pendingClaims = _dbContext.Claims
                .Include(c => c.ApplicationUser)
                .Include(c => c.Documents)
                .Where(c => !c.IsApprovedByCoordinator && c.Status == "Pending")
                .ToList();

            return View(pendingClaims);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int claimId)
        {
            var claim = await _dbContext.Claims.FindAsync(claimId);

            if (claim != null)
            {
                claim.IsApprovedByCoordinator = true;
                claim.Status = "Approved by Coordinator";
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int claimId)
        {
            var claim = await _dbContext.Claims.FindAsync(claimId);

            if (claim != null)
            {
                claim.Status = "Rejected by Coordinator";
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
