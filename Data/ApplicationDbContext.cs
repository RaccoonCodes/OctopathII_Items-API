using Microsoft.EntityFrameworkCore;
using OctopathII_Items.Models;

namespace OctopathII_Items.Data
{
    public class ApplicationDbContext: DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Equipment> Equipment => Set<Equipment>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
