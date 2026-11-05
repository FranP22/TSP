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

        public virtual DbSet<AppUser> Users { get; set; }
        public virtual DbSet<AppRole> Roles { get; set; }
        public virtual DbSet<AppRoleClaim> RoleClaims { get; set; }
        public virtual DbSet<AppUserSession> UserSessions { get; set; }
        public virtual DbSet<AppUsedRefreshTokens> UsedRefreshTokens { get; set; }
        public virtual DbSet<AppOrder> Orders { get; set; }
        public virtual DbSet<AppProduct> Products { get; set; }
    }
}
