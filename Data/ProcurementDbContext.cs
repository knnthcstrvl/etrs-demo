using Financial.Model;
using Financial.Model.Dtr;
// Procurement and Trace models removed from this project; only DTR models are used here.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace Financial.Data
{
    // Renamed to EtrsDbContext and trimmed to project-specific entities
    public class EtrsDbContext : DbContext
    {
        public EtrsDbContext(DbContextOptions<EtrsDbContext> options)
            : base(options)
        {
        }

        // Core / auth
        public DbSet<UserAccount> UserAccounts { get; set; }

        // DTR (attendance)
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public DbSet<AttendanceLogWfh> AttendanceLogsWfh { get; set; }
        public DbSet<ExceptionCondition> ExceptionConditions { get; set; }

        // Removed procurement/financial DbSet properties as these types belong to another project
        // and were accidentally mixed into this repository. Keep only attendance/auth entities here.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.IdNumber)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .Property(e => e.DivisionCenterUnit)
                .HasColumnName("DivisionCenterAdminUnit");

            modelBuilder.Entity<AttendanceLog>()
                .Property(l => l.DivisionCenterUnit)
                .HasColumnName("DivisionCenterAdminUnit");

            modelBuilder.Entity<AttendanceLog>()
                .Property(l => l.Tardiness)
                .HasColumnName("LateUndertime");

            modelBuilder.Entity<AttendanceLogWfh>()
                .Property(l => l.DivisionCenterUnit)
                .HasColumnName("DivisionCenterAdminUnit");

            modelBuilder.Entity<AttendanceLogWfh>()
                .Property(l => l.Tardiness)
                .HasColumnName("LateUndertime");

            modelBuilder.Entity<UserAccount>()
                .Property(u => u.DivisionCenterUnit)
                .HasColumnName("DivisionCenterAdminUnit")
                .HasMaxLength(1000);

            modelBuilder.Entity<AttendanceLog>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.AttendanceLogs)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttendanceLogWfh>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.AttendanceLogsWfh)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttendanceLogWfh>()
                .HasOne(l => l.ExceptionCondition)
                .WithMany(c => c.AttendanceLogsWfh)
                .HasForeignKey(l => l.ExceptionConditionId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AttendanceLog>()
                .HasIndex(l => new { l.EmployeeId, l.AttendanceDate });

            modelBuilder.Entity<AttendanceLogWfh>()
                .HasIndex(l => new { l.EmployeeId, l.AttendanceDate });

            // Ensure decimal properties have an explicit precision/scale to avoid
            // SQL Server truncation warnings and EF warnings during migrations.
            // Applies a default precision of (18,2) to all decimal properties.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var decimalProps = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

                foreach (var prop in decimalProps)
                {
                    // Use SetPrecision/SetScale which maps to HasPrecision(precision, scale)
                    prop.SetPrecision(18);
                    prop.SetScale(2);
                }
            }
        }
    }
}
