using Microsoft.EntityFrameworkCore;
using ChatBackend.Models;

namespace ChatBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        public DbSet<Message> Messages { get; set; }
    }
}
