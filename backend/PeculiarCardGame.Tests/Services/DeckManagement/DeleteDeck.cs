using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.UnitTests.Services.DeckManagement
{
    public class DeleteDeck
    {
        const int NotExistingDeckId = 2;

        private readonly Deck _deck;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _authorFilledRequestContext;
        private readonly RequestContext _notAuthorFilledRequestContext;

        public DeleteDeck()
        {
            const int AuthorId = 1;
            const int NotAuthorId = 2;
            const string Username = "test";
            const string DisplayedName = "test";
            const string DeckName = "test";
            const string DeckDescription = "test";

            const int ExistingDeckId = 1;

            _deck = new Deck
            {
                Id = ExistingDeckId,
                AuthorId = AuthorId,
                Name = DeckName,
                Description = DeckDescription
            };

            _dbContext = TestHelpers.GetDbContext();

            _emptyRequestContext = new RequestContext();
            _authorFilledRequestContext = new RequestContext();
            _authorFilledRequestContext.SetOnce(new User
            {
                Id = AuthorId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _notAuthorFilledRequestContext = new RequestContext();
            _notAuthorFilledRequestContext.SetOnce(new User
            {
                Id = NotAuthorId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _emptyRequestContext);

            var action = () => service.DeleteDeck(_deck.Id);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldNotDeleteDeck()
        {
            _dbContext.SetupTest(_deck);
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
                service.DeleteDeck(_deck.Id);
            }
            catch { }

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }

        [Fact]
        public void NotExistingDeck_ShouldReturnFalse()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var deleted = service.DeleteDeck(NotExistingDeckId);

            deleted.Should().BeFalse();
        }

        [Fact]
        public void NotExistingDeck_ShouldNotDeleteDeck()
        {
            _dbContext.SetupTest(_deck);
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.DeleteDeck(NotExistingDeckId);
            var deck = _dbContext.Decks.Single(x => x.Id == _deck.Id);

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
            deck.Id.Should().Be(_deck.Id);
            deck.AuthorId.Should().Be(_deck.AuthorId);
            deck.Name.Should().Be(_deck.Name);
            deck.Description.Should().Be(_deck.Description);
        }

        [Fact]
        public void NotAuthor_ShouldReturnFalse()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            var deleted = service.DeleteDeck(_deck.Id);

            deleted.Should().BeFalse();
        }

        [Fact]
        public void NotAuthor_ShouldNotDeleteDeck()
        {
            _dbContext.SetupTest(_deck);
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            service.DeleteDeck(_deck.Id);
            var deck = _dbContext.Decks.Single(x => x.Id == _deck.Id);

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
            deck.Id.Should().Be(_deck.Id);
            deck.AuthorId.Should().Be(_deck.AuthorId);
            deck.Name.Should().Be(_deck.Name);
            deck.Description.Should().Be(_deck.Description);
        }

        [Fact]
        public void ExistingDeck_ShouldReturnTrue()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var deleted = service.DeleteDeck(_deck.Id);

            deleted.Should().BeTrue();
        }

        [Fact]
        public void ExistingDeck_ShouldDeleteDeck()
        {
            _dbContext.SetupTest(_deck);
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.DeleteDeck(_deck.Id);
            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == _deck.Id);

            deck.Should().BeNull();
            _dbContext.Decks.Should().HaveCount(deckCountBefore - 1);
        }
    }
}
