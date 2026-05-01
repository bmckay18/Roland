using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
    }
}
