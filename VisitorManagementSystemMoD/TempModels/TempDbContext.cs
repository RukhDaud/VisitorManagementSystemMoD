using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VisitorManagementSystemMoD.TempModels;

public partial class TempDbContext : DbContext
{
    public TempDbContext(DbContextOptions<TempDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Visitor> Visitors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.DepartmentId, "IX_Users_DepartmentId");

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasDefaultValue("");

            entity.HasOne(d => d.Department).WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasIndex(e => e.ApprovedById, "IX_Visitors_ApprovedById");

            entity.HasIndex(e => e.DepartmentId, "IX_Visitors_DepartmentId");

            entity.HasIndex(e => e.EmployeeId, "IX_Visitors_EmployeeId");

            entity.Property(e => e.ApprovedByName).HasMaxLength(100);
            entity.Property(e => e.Cnic)
                .HasMaxLength(20)
                .HasColumnName("CNIC");
            entity.Property(e => e.EmployeeName).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Purpose).HasMaxLength(500);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.VehicleNumber).HasMaxLength(50);
            entity.Property(e => e.VehicleType).HasMaxLength(50);

            entity.HasOne(d => d.ApprovedBy).WithMany(p => p.VisitorApprovedBies).HasForeignKey(d => d.ApprovedById);

            entity.HasOne(d => d.Department).WithMany(p => p.Visitors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Employee).WithMany(p => p.VisitorEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
