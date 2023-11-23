using Microsoft.EntityFrameworkCore;
using LoggingMicroservice.Models;

namespace LoggingMicroservice.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {
            Database.EnsureCreated();
        }

        public DbSet<Log>? Logs { get; set; }
    }
}
