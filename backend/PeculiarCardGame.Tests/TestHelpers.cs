using Microsoft.EntityFrameworkCore;
using PeculiarCardGame.Data;

namespace PeculiarCardGame.UnitTests
{
    internal static class TestHelpers
    {
        public static PeculiarCardGameDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<PeculiarCardGameDbContext>()
                .UseLazyLoadingProxies()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new PeculiarCardGameDbContext(options);

            return dbContext;
        }
    }
}
