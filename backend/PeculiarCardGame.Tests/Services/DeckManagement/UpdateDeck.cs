using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class UpdateDeck
    {
        private const string NewName = "new";
        private const string NewDescription = "new";

        private readonly Deck _deck;
        private readonly Deck _anotherDeck;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _authorFilledRequestContext;
        private readonly RequestContext _notAuthorFilledRequestContext;

        public UpdateDeck()
        {
            const int AuthorId = 1;
            const int NotAuthorId = 2;
            const string Username = "test";
            const string DisplayedName = "test";
            const int DeckId = 1;
            const int AnotherDeckId = 2;
            const string DeckName = "test";
            const string DeckDescription = "test";

            _deck = new Deck
            {
                AuthorId = AuthorId,
                Id = DeckId,
                Name = DeckName,
                Description = DeckDescription
            };
            _anotherDeck = new Deck
            {
                AuthorId = AuthorId,
                Id = AnotherDeckId,
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

            var action = () => service.UpdateDeck(_deck.Id, NewName, NewDescription);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldNotUpdateDeck()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
                service.UpdateDeck(_deck.Id, NewName, NewDescription);
            }
            catch (InvalidOperationException) { }
            var deck = _dbContext.Decks.Single(x => x.Id == _deck.Id);

            deck.AuthorId.Should().Be(_deck.AuthorId);
            deck.Id.Should().Be(_deck.Id);
            deck.Name.Should().Be(_deck.Name);
            deck.Description.Should().Be(_deck.Description);
        }

        [Fact]
        public void NotAuthor_ShouldReturnNull()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            var deck = service.UpdateDeck(_deck.Id, NewName, NewDescription);

            deck.Should().BeNull();
        }

        [Fact]
        public void NotAuthor_ShouldNotUpdateDeck()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            service.UpdateDeck(_deck.Id, NewName, NewDescription);
            var deck = _dbContext.Decks.Single(x => x.Id == _deck.Id);

            deck.AuthorId.Should().Be(_deck.AuthorId);
            deck.Id.Should().Be(_deck.Id);
            deck.Name.Should().Be(_deck.Name);
            deck.Description.Should().Be(_deck.Description);
        }

        [Fact]
        public void NotExistingDeckId_ShouldReturnNull()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var deck = service.UpdateDeck(_anotherDeck.Id, NewName, NewDescription);

            deck.Should().BeNull();
        }

        [Fact]
        public void NotExistingDeckId_ShouldNotUpdateDeck()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.UpdateDeck(_anotherDeck.Id, NewName, NewDescription);
            var deck = _dbContext.Decks.Single(x => x.Id == _deck.Id);

            deck.AuthorId.Should().Be(_deck.AuthorId);
            deck.Id.Should().Be(_deck.Id);
            deck.Name.Should().Be(_deck.Name);
            deck.Description.Should().Be(_deck.Description);
        }

        [Theory]
        [InlineData(NewName, null)]
        [InlineData(null, NewDescription)]
        public void ExistingDeckId_ShouldReturnUpdatedDeck(string? nameUpdate, string? descriptionUpdate)
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var deck = service.UpdateDeck(_deck.Id, nameUpdate, descriptionUpdate);

            deck.Should().NotBeNull();
            deck!.AuthorId.Should().Be(_deck.AuthorId);
            deck.Id.Should().Be(_deck.Id);
            deck.Name.Should().Be(nameUpdate ?? _deck.Name);
            deck.Description.Should().Be(descriptionUpdate ?? _deck.Description);
        }

        [Theory]
        [InlineData(NewName, null)]
        [InlineData(null, NewDescription)]
        public void ExistingDeckId_ShouldUpdateDeck(string? nameUpdate, string? descriptionUpdate)
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.UpdateDeck(_deck.Id, nameUpdate, descriptionUpdate);
            var deck = _dbContext.Decks.Single(x => x.Id == _deck.Id);

            deck.AuthorId.Should().Be(_deck.AuthorId);
            deck.Id.Should().Be(_deck.Id);
            deck.Name.Should().Be(nameUpdate ?? _deck.Name);
            deck.Description.Should().Be(descriptionUpdate ?? _deck.Description);
        }

        [Fact]
        public void ExistingDeckId_ShouldNotChangeDeckCount()
        {
            _dbContext.SetupTest(_deck);
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.UpdateDeck(_deck.Id, _deck.Name, _deck.Description);

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }
    }
}
