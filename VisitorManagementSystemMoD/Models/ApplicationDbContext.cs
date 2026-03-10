using Microsoft.EntityFrameworkCore;

namespace VisitorManagementSystemMoD.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<BlockedVisitor> BlockedVisitors { get; set; }
        public DbSet<DepartmentEmployee> DepartmentEmployees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Visitor>()
                .HasOne(v => v.Employee)
                .WithMany(u => u.Visitors)
                .HasForeignKey(v => v.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visitor>()
                .HasOne(v => v.ApprovedBy)
                .WithMany()
                .HasForeignKey(v => v.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visitor>()
                .HasOne(v => v.Department)
                .WithMany(d => d.Visitors)
                .HasForeignKey(v => v.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Alert>()
                .HasOne(a => a.CreatedBy)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BlockedVisitor>()
                .HasOne(b => b.BlockedBy)
                .WithMany()
                .HasForeignKey(b => b.BlockedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DepartmentEmployee>()
                .HasOne(de => de.User)
                .WithMany(u => u.DepartmentEmployees)
                .HasForeignKey(de => de.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visitor>()
                .HasOne(v => v.DepartmentEmployee)
                .WithMany()
                .HasForeignKey(v => v.DepartmentEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed default SuperAdmin role
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "SuperAdmin",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed default SuperAdmin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "System Administrator",
                    Username = "superadmin",
                    Password = "super123", // In production, this should be hashed
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
