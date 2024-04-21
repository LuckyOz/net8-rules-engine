
using Microsoft.EntityFrameworkCore;
using net8_rules_engine.Models.Entities;

namespace net8_rules_engine.Contexts
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<VersionEngine> VersionEngines { get; set; }
        public DbSet<VersionEngineDetail> VersionEngineDetails { get; set; }
        public DbSet<DataEngine> DataEngines { get; set; }
    }
}
