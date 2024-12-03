using Libereay_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Libereay_System.Entity
{
    public class LibraryDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; }
        public DbSet<FineConfiguration> FineConfigurations { get; set; }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=MAZENAHMED;Database=Library;Trusted_Connection=True;Encrypt=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(bt => bt.Book)
                .WithMany()
                .HasForeignKey(bt => bt.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(bt => bt.User)
                .WithMany()
                .HasForeignKey(bt => bt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FineConfiguration>().HasData(
                new FineConfiguration
                {
                    Id = 1,
                    FinePerDay = 2.00m // Default fine policy
                }
            );
        }
    }
}
