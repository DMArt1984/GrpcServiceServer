using Microsoft.EntityFrameworkCore;

namespace GrpcServicePiter
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Worker> Workers { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Worker>().HasData(
                    new Worker { Id = 1, LastName = "Black", FirstName = "Kevin" },
                    new Worker { Id = 2, LastName = "White", FirstName = "Martin", BirthDay = "2000.11.12" },
                    new Worker { Id = 3, LastName = "Zegers", FirstName = "Anna", Sex = true, BirthDay = "2003.01.01" },
                    new Worker { Id = 4, LastName = "Maximov", FirstName = "Max", HaveChildren = true }
            );
        }
    }

}
