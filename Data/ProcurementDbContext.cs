using Financial.Model.Library;
using Financial.Model.Modules;
using Financial.Model.Trace;
using Financial.Model;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Financial.Data
{
    public class ProcurementDbContext : DbContext
    {
        public ProcurementDbContext(DbContextOptions<ProcurementDbContext> options)
            : base(options)
        {
        }

        // Library
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ProcurementMode> ProcurementModes { get; set; }
        public DbSet<ProcurementType> ProcurementTypes { get; set; }
        public DbSet<FundCluster> FundClusters { get; set; }
        public DbSet<PaymentMode> PaymentModes { get; set; }

        // Core
        public DbSet<Transaction> Transactions { get; set; }

        // Modules
        public DbSet<Procurement> Procurements { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Accounting> Accountings { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }

        // Trace/Docu
        public DbSet<Document> Documents { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }

        //User Account
        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================
            // TRANSACTION
            // ==========================
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.PrNumber)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Supplier)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // ONE-TO-ONE MODULES
            // ==========================
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

            // ==========================
            // LOOKUP CONSTRAINTS
            // ==========================
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
        }
    }
}
