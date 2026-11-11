using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TSP.Data;

namespace TSP.Test.Tests
{
    public abstract class BaseTest
    {
        protected AppDbContext _db;

        [SetUp]
        public void SetUpDatabase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _db = new AppDbContext(options);
        }

        [TearDown]
        public void TearDownDatabase()
        {
            _db?.Dispose();
        }
    }
}
