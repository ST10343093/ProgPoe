using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgPoe.Models;

namespace ProgPoe.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Document> Documents { get; set; }

        public DbSet<Claim> Claims { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
