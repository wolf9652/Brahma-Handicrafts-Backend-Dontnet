using Microsoft.EntityFrameworkCore;
using BRAHMA_HANDICRAFT_BACKEND.Domain; // reference your models

namespace BRAHMA_HANDICRAFT_BACKEND.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Example table
        public DbSet<Users> Users { get; set; }
        public DbSet<UsersCredentials> UsersCredentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Explicitly set PKs if needed
            modelBuilder.Entity<Users>().HasKey(u => u.UserId);
            modelBuilder.Entity<UsersCredentials>().HasKey(c => c.CredentialId);

            // Relationship: UserCredentials → User
            modelBuilder.Entity<UsersCredentials>()
                .HasOne(c => c.User)
                .WithMany() // one user can have many credentials if you want
                .HasForeignKey(c => c.UserId);
        }
    }
}
