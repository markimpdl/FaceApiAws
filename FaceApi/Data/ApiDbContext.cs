using FaceApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace FaceApi.Data
{
    public class ApiDbContext : IdentityDbContext<AdministratorUser>
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<UserSchool> UserSchools { get; set; }
        public DbSet<PresenceRecord> PresenceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserSchool>()
                .HasKey(us => new { us.UserId, us.SchoolId });
            builder.Entity<UserSchool>()
                .HasOne(us => us.User).WithMany(u => u.UserSchools).HasForeignKey(us => us.UserId);
            builder.Entity<UserSchool>()
                .HasOne(us => us.School).WithMany(s => s.UserSchools).HasForeignKey(us => us.SchoolId);
        }
    }

}