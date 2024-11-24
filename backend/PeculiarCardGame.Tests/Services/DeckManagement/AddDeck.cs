using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class AddDeck
    {
        private const int UserId = 1;
        private const string Username = "test";
        private const string DisplayedName = "test";

        private const string DeckName = "test";
        private const string DeckDescription = "test";

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public AddDeck()
        {
            _dbContext = TestHelpers.GetDbContext();

            _emptyRequestContext = new RequestContext();
            _filledRequestContext = new RequestContext();
            _filledRequestContext.SetOnce(new User
            {
                Id = UserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
        }

        [Fact]
        public void NullName_ShouldThrowArgumentNullException()
        {
            var service = new Service(_dbContext, _filledRequestContext);

#pragma warning disable CS8625
            var action = () => service.AddDeck(null, DeckDescription);
#pragma warning restore CS8625

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NullName_ShouldNotAddDeck()
        {
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            try
            {
#pragma warning disable CS8625
                service.AddDeck(null, DeckDescription);
#pragma warning restore CS8625
            }
            catch (ArgumentNullException) { }

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            var service = new Service(_dbContext, _emptyRequestContext);

            var action = () => service.AddDeck(DeckName, DeckDescription);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldNotAddDeck()
        {
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
                service.AddDeck(DeckName, DeckDescription);
            }
            catch (InvalidOperationException) { }

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }

        [Fact]
        public void FilledRequestContext_ShouldAddDeck()
        {
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.AddDeck(DeckName, DeckDescription);

            _dbContext.Decks.Should().HaveCount(deckCountBefore + 1);
        }

        [Fact]
        public void FilledRequestContext_ShouldReturnNewDeck()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var deck = service.AddDeck(DeckName, DeckDescription);

            deck.Name.Should().Be(DeckName);
            deck.Description.Should().Be(DeckDescription);
            deck.AuthorId.Should().Be(UserId);
        }

        [Fact]
        public void NullDescription_ShouldAddDeckWithEmptyStringAsDescription()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var deck = service.AddDeck(DeckName, null);

            deck.Description.Should().Be("");
        }
    }
}
