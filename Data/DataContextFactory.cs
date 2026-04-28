using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "roland_migration.db");

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new DataContext(options);
        }
    }
}
