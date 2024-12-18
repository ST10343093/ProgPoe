﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgPoe.Data;

namespace ProgPoe.Controllers
{
    // This controller is restricted to users with the "Manager" role
    [Authorize(Roles = "Manager")]
    public class AcademicManagerController : Controller
    {
      
            private readonly ApplicationDbContext _context;

            public AcademicManagerController(ApplicationDbContext context)
            {
                _context = context;
            }

            public IActionResult Index()
            {
                var pendingClaims = _context.Claims
                    .Include(c => c.ApplicationUser)
                    .Include(c => c.Documents)
                    .Where(c => c.IsApprovedByCoordinator && !c.IsApprovedByManager && c.Status == "Approved by Coordinator")
                    .ToList();

                return View(pendingClaims);
            }

            [HttpPost]
            public async Task<IActionResult> Approve(int claimId)
            {
                var claim = await _context.Claims.FindAsync(claimId);

                if (claim != null)
                {
                    claim.IsApprovedByManager = true;
                    claim.Status = "Approved by Manager";
                    claim.PaymentStatus = "Processing";
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            [HttpPost]
            public async Task<IActionResult> Reject(int claimId)
            {
                var claim = await _context.Claims.FindAsync(claimId);

                if (claim != null)
                {
                    claim.Status = "Rejected by Manager";
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
        }
    }
