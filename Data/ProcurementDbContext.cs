using Financial.Model;
using Financial.Model.Dtr;
using Financial.Model.Library;
using Financial.Model.Modules;
using Financial.Model.Trace;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data
{
    public class ProcurementDbContext : DbContext
    {
        public ProcurementDbContext(DbContextOptions<ProcurementDbContext> options)
            : base(options)
        {
        }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ProcurementMode> ProcurementModes { get; set; }
        public DbSet<ProcurementType> ProcurementTypes { get; set; }
        public DbSet<FundCluster> FundClusters { get; set; }
        public DbSet<PaymentMode> PaymentModes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Procurement> Procurements { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Accounting> Accountings { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<BiometricDevice> BiometricDevices { get; set; }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public DbSet<AttendanceLogWfh> AttendanceLogsWfh { get; set; }
        public DbSet<ExceptionCondition> ExceptionConditions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.PrNumber)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Supplier)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Procurement)
                .WithOne(p => p.Transaction)
                .HasForeignKey<Procurement>(p => p.TransactionId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Budget)
                .WithOne(b => b.Transaction)
                .HasForeignKey<Budget>(b => b.TransactionId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Property)
                .WithOne(p => p.Transaction)
                .HasForeignKey<Property>(p => p.TransactionId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Accounting)
                .WithOne(a => a.Transaction)
                .HasForeignKey<Accounting>(a => a.TransactionId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Cashier)
                .WithOne(c => c.Transaction)
                .HasForeignKey<Cashier>(c => c.TransactionId);

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.SupplierName)
                .IsUnique();

            modelBuilder.Entity<ProcurementMode>()
                .HasIndex(m => m.ModeName)
                .IsUnique();

            modelBuilder.Entity<ProcurementType>()
                .HasIndex(t => t.TypeName)
                .IsUnique();

            modelBuilder.Entity<FundCluster>()
                .HasIndex(f => f.FundClusterName)
                .IsUnique();

            modelBuilder.Entity<PaymentMode>()
                .HasIndex(p => p.PaymentModeName)
                .IsUnique();

            modelBuilder.Entity<UserAccount>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserAccount>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.IdNumber)
                .IsUnique();

            modelBuilder.Entity<BiometricDevice>()
                .HasIndex(d => d.SerialNumber)
                .IsUnique();

            modelBuilder.Entity<AttendanceLog>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.AttendanceLogs)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttendanceLog>()
                .HasOne(l => l.BiometricDevice)
                .WithMany(d => d.AttendanceLogs)
                .HasForeignKey(l => l.BiometricDeviceId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AttendanceLogWfh>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.AttendanceLogsWfh)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttendanceLog>()
                .HasIndex(l => new { l.EmployeeId, l.AttendanceDate });

            modelBuilder.Entity<AttendanceLogWfh>()
                .HasIndex(l => new { l.EmployeeId, l.AttendanceDate });
        }
    }
}
