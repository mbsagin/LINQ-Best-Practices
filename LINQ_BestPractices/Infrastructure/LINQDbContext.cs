using Microsoft.EntityFrameworkCore;

namespace LINQ_BestPractices.Infrastructure
{
    public class LINQDbContext : DbContext
    {
        public virtual DbSet<Models.User> Users { get; set; }
    }
}
