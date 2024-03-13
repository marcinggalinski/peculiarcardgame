using FluentAssertions;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Shared.Options;
using DbContext = PeculiarCardGame.Data.PeculiarCardGameDbContext;

namespace PeculiarCardGame.UnitTests.Data
{
    public class PeculiarCardGameDbContext
    {
        [Fact]
        public void Contructor_BothNulls_ShouldThrowArgumentException()
        {
            var action = () => new DbContext(null, null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Contructor_BothNotNulls_ShouldThrowArgumentException()
        {
            var action = () => new DbContext(Options.Create(new DbOptions { ConnectionString = "" }), _ => { });

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Contructor_OptionsNotNull_ShouldNotThrowAnyException()
        {
            var action = () => new DbContext(Options.Create(new DbOptions { ConnectionString = "" }), null);

            action.Should().NotThrow();
        }

        [Fact]
        public void Contructor_ActionNotNull_ShouldNotThrowAnyException()
        {
            var action = () => new DbContext(null, _ => { });

            action.Should().NotThrow();
        }
    }
}
