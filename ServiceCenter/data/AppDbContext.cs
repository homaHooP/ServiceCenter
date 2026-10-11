using Microsoft.EntityFrameworkCore;

namespace ServiceCenter.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Models.ServiceRequest> ServiceRequests { get; set; } = null!;
    }
}
