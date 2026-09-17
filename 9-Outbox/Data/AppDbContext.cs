using _9_Outbox.Models;
using Microsoft.EntityFrameworkCore;

namespace _9_Outbox.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    }
}
