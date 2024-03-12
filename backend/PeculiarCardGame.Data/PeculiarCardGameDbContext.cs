using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Options;

namespace PeculiarCardGame.Data
{
    public class PeculiarCardGameDbContext : DbContext
    {
        private readonly DbOptions? _sqlServerOptions;
        private readonly Action<DbContextOptionsBuilder>? _optionsAction;

        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Deck> Decks { get; set; }

        public PeculiarCardGameDbContext(IOptions<DbOptions>? sqlServerOptions, Action<DbContextOptionsBuilder>? optionsAction = null)
        {
            if (sqlServerOptions?.Value is null && optionsAction is null)
                throw new ArgumentException($"Either {nameof(sqlServerOptions)} or {nameof(optionsAction)} must not be null.");
            if (sqlServerOptions?.Value is not null && optionsAction is not null)
                throw new ArgumentException($"Only either {nameof(sqlServerOptions)} or {nameof(optionsAction)} must be provided.");

            _sqlServerOptions = sqlServerOptions?.Value;
            _optionsAction = optionsAction;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            if (_optionsAction is not null)
            {
                _optionsAction(optionsBuilder);
                return;
            }

            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlServer(_sqlServerOptions!.ConnectionString);
        }
    }
}
