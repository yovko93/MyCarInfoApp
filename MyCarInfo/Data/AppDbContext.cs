using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyCarInfo.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        {
        }

        public DbSet<Vehicle> Cars { get; set; }
        public DbSet<CarImage> CarImages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
