using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Db;
using TSP.Data.Extensions;

namespace TSP.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.CreateAppModel();
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppRole> Roles { get; set; }
        public DbSet<AppRoleClaim> RoleClaims { get; set; }
        public DbSet<AppUserSession> UserSessions { get; set; }
        public DbSet<AppUsedRefreshTokens> UsedRefreshTokens { get; set; }
        public DbSet<AppOrder> Orders { get; set; }
        public DbSet<AppProduct> Products { get; set; }
    }
}
