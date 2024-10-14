using Microsoft.EntityFrameworkCore;

namespace ProgPoe.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ProgPoe.Models.Claim> Claim { get; set; } = default!;
    }
}
