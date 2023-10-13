using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Options;

namespace PeculiarCardGame.Data
{
    public class PeculiarCardGameDbContext : DbContext
    {
        private readonly DbOptions? _sqlServerOptions;

        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Deck> Decks { get; set; }

        public PeculiarCardGameDbContext(IOptions<DbOptions> sqlServerOptions)
        {
            _sqlServerOptions = sqlServerOptions.Value;
        }

        public PeculiarCardGameDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlServer(_sqlServerOptions!.ConnectionString);
        }
    }
}
