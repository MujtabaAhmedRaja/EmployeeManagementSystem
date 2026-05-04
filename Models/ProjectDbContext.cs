using Microsoft.EntityFrameworkCore;

namespace EMS.Models;

public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
        : base(options)
    {
    }

    public DbSet<Attendance> Attendances { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<LeaveTable> LeaveTables { get; set; } = null!;
    public DbSet<Log> Logs { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Salary> Salaries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("Attendance", tb => tb.HasTrigger("tr_Attendance"));
            entity.HasKey(e => e.AttId);
            entity.Property(e => e.AttId).HasColumnName("att_id");
            entity.Property(e => e.Eid).HasColumnName("eid");
            entity.Property(e => e.AttDate).HasColumnName("att_date");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(10);
            entity.HasOne(e => e.Employee)
                  .WithMany(e => e.Attendances)
                  .HasForeignKey(e => e.Eid)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department", tb => tb.HasTrigger("tr_Department"));
            entity.HasKey(e => e.DepId);
            entity.Property(e => e.DepId).HasColumnName("dep_id");
            entity.Property(e => e.DepName).HasColumnName("dep_name").HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee", tb => tb.HasTrigger("tr_Employee"));
            entity.HasKey(e => e.Eid);
            entity.Property(e => e.Eid).HasColumnName("eid");
            entity.Property(e => e.EName).HasColumnName("eName").HasMaxLength(100);
            entity.Property(e => e.EAge).HasColumnName("eAge");
            entity.Property(e => e.ECity).HasColumnName("eCity").HasMaxLength(100);
            entity.Property(e => e.DepId).HasColumnName("dep_id");
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Employees)
                  .HasForeignKey(e => e.DepId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<LeaveTable>(entity =>
        {
            entity.ToTable("LeaveTable", tb => tb.HasTrigger("tr_LeaveTable"));
            entity.HasKey(e => e.LeaveId);
            entity.Property(e => e.LeaveId).HasColumnName("leave_id");
            entity.Property(e => e.Eid).HasColumnName("eid");
            entity.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(200);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
            entity.HasOne(e => e.Employee)
                  .WithMany(e => e.LeaveRequests)
                  .HasForeignKey(e => e.Eid)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log", tb => tb.HasTrigger("tr_Log"));
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TableName).HasColumnName("table_name").HasMaxLength(100);
            entity.Property(e => e.Action).HasColumnName("action").HasMaxLength(500);
            entity.Property(e => e.ActionDate).HasColumnName("action_date");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role", tb => tb.HasTrigger("tr_Role"));
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName).HasColumnName("role_name").HasMaxLength(50);
        });

        modelBuilder.Entity<Salary>(entity =>
        {
            entity.ToTable("Salary", tb => tb.HasTrigger("tr_Salary"));
            entity.HasKey(e => new { e.Eid, e.RoleId });
            entity.Property(e => e.Eid).HasColumnName("eid");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.HasOne(e => e.Employee)
                  .WithMany(e => e.SalaryRecords)
                  .HasForeignKey(e => e.Eid)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Role)
                  .WithMany(r => r.SalaryRecords)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
