using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Options;

namespace PeculiarCardGame.Data
{
    public class PeculiarCardGameDbContext : DbContext
    {
        private readonly SqlServerOptions _sqlServerOptions;

        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Deck> Decks { get; set; }

        public PeculiarCardGameDbContext(IOptions<SqlServerOptions> sqlServerOptions)
        {
            _sqlServerOptions = sqlServerOptions.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlServer(_sqlServerOptions.ConnectionString);
        }
    }
}
