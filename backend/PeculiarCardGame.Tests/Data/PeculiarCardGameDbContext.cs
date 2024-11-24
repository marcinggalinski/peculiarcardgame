using FluentAssertions;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Shared.Options;
using DbContext = PeculiarCardGame.Data.PeculiarCardGameDbContext;

namespace PeculiarCardGame.Tests.Data
{
    public class PeculiarCardGameDbContext
    {
        [Fact]
        public void Constructor_BothNulls_ShouldThrowArgumentException()
        {
            var action = () => new DbContext(null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_BothNotNulls_ShouldThrowArgumentException()
        {
            var action = () => new DbContext(Options.Create(new DbOptions { ConnectionString = "" }), _ => { });

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_OptionsNotNull_ShouldNotThrowAnyException()
        {
            var action = () => new DbContext(Options.Create(new DbOptions { ConnectionString = "" }));

            action.Should().NotThrow();
        }

        [Fact]
        public void Constructor_ActionNotNull_ShouldNotThrowAnyException()
        {
            var action = () => new DbContext(null, _ => { });

            action.Should().NotThrow();
        }
    }
}
