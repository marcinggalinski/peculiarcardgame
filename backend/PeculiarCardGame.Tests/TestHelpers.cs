using Microsoft.EntityFrameworkCore;
using PeculiarCardGame.Data;

namespace PeculiarCardGame.UnitTests
{
    internal static class TestHelpers
    {
        public static PeculiarCardGameDbContext GetDbContext()
        {
            var dbContext = new PeculiarCardGameDbContext(null, builder =>
            {
                builder.UseLazyLoadingProxies();
                builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            return dbContext;
        }
    }
}
