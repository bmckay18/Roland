using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public virtual DbSet<Distribution> Distributions { get; set; }
        public virtual DbSet<ParcelAllocation> ParcelAllocations { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParcelAllocation>()
                .HasOne(p => p.BuyTransaction)
                .WithMany()
                .HasForeignKey(p => p.BuyTransactionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParcelAllocation>()
                .HasOne(p => p.SellTransaction)
                .WithMany()
                .HasForeignKey(p => p.SellTransactionID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
