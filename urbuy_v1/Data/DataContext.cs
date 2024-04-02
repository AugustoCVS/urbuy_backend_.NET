using Microsoft.EntityFrameworkCore;
using urbuy_v1.Models;

namespace urbuy_v1.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }
        public DbSet<AdmUser> AdmUsers { get; set; }
    }
}
