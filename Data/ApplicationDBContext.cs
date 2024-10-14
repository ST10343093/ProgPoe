﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgPoe.Models;

namespace ProgPoe.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ProgPoe.Models.Claim> Claim { get; set; } = default!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
