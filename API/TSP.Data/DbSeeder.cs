using Microsoft.EntityFrameworkCore;

namespace TSP.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await db.Database.MigrateAsync();
        }
    }
}
