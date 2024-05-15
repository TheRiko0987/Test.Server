using Microsoft.EntityFrameworkCore;

namespace Test.Server
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Person> Persons { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasData(
                new Person { Id = 1, Name = "Тамара", Age = 18 },
                new Person { Id = 2, Name = "Женя", Age = 19 },
                new Person { Id = 3, Name = "Анна", Age = 20 }
                );
        }*/
    }
}
