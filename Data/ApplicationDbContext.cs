using MainProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MainProject.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Change to be your model(s) and table(s)
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}