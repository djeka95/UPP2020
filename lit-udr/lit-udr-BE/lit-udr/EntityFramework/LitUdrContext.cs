using lit_udr.EntityFramework.Model;
using Microsoft.EntityFrameworkCore;

namespace lit_udr.EntityFramework
{
    public class LitUdrContext:DbContext
    {
        public LitUdrContext(DbContextOptions<LitUdrContext> options) :base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<UserGenre> UserGenres { get; set; }
        public DbSet<UserReview> UserReview { get; set; }
        public DbSet<NewUserData> NewUserData { get; set; }
        public DbSet<WorkApplicationData> workApplicationData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGenre>()
                .HasKey(bc => new { bc.UserId, bc.GenreId });
            modelBuilder.Entity<UserGenre>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserGenres)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<UserGenre>()
                .HasOne(bc => bc.Genre)
                .WithMany(c => c.UserGenres)
                .HasForeignKey(bc => bc.GenreId);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserReview>()
    .HasKey(bc => new { bc.UserId, bc.WorkApplicationDataId });
            modelBuilder.Entity<UserReview>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.WorkApplicationData)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<UserReview>()
                .HasOne(bc => bc.WorkApplicationData)
                .WithMany(c => c.BoardMembers)
                .HasForeignKey(bc => bc.WorkApplicationDataId);
        }
    }
}
