using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service.Execution;

namespace Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
