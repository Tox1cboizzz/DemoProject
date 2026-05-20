using Microsoft.EntityFrameworkCore;

namespace BillingService.Entity
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Bill> Bills { get; set; }
    }
}