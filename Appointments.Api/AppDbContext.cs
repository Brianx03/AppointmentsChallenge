using Appointments.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Api
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(user => user.Appointments)
                .WithOne(user => user.User)
                .HasForeignKey(appointment => appointment.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
