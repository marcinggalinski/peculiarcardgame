using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.UnitTests.Services.DeckManagement
{
    public class AddCard
    {
        private const string CardText = "test";
        private const CardType CardType = PeculiarCardGame.CardType.Black;

        private readonly Deck _deck;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _authorFilledRequestContext;
        private readonly RequestContext _notAuthorFilledRequestContext;

        public AddCard()
        {
            const int DeckId = 1;
            const int AuthorId = 1;
            const int NotAuthorId = 2;
            const string DeckName = "test";
            const string DeckDescription = "test";
            const string AuthorUsername = "test";
            const string DisplayedName = "test";
            const string NotAuthorUsername = "test";

            _deck = new Deck
            {
                Id = DeckId,
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
                Username = AuthorUsername,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _notAuthorFilledRequestContext = new RequestContext();
            _notAuthorFilledRequestContext.SetOnce(new User
            {
                Id = NotAuthorId,
                Username = NotAuthorUsername,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            var service = new Service(_dbContext, _emptyRequestContext);

            var action = () => service.AddCard(_deck.Id, CardText, CardType);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldNotAddCard()
        {
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
                service.AddCard(_deck.Id, CardText, CardType);
            }
            catch { }

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
        }

        [Fact]
        public void NullText_ShouldThrowArgumentNullException()
        {
            var service = new Service(_dbContext, _authorFilledRequestContext);

#pragma warning disable CS8625
            var action = () => service.AddCard(_deck.Id, null, CardType);
#pragma warning restore CS8625

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NullText_ShouldNotAddCard()
        {
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _authorFilledRequestContext);

            try
            {
#pragma warning disable CS8625
                service.AddCard(_deck.Id, null, CardType);
#pragma warning restore CS8625
            }
            catch { }

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
        }

        [Fact]
        public void NotExistingDeckId_ShouldReturnNull()
        {
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var card = service.AddCard(_deck.Id, CardText, CardType);

            card.Should().BeNull();
        }

        [Fact]
        public void NotExistingDeck_ShouldNotAddCard()
        {
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var card = service.AddCard(_deck.Id, CardText, CardType);

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
        }

        [Fact]
        public void NotAuthor_ShouldReturnNull()
        {
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            var card = service.AddCard(_deck.Id, CardText, CardType);

            card.Should().BeNull();
        }

        [Fact]
        public void NotAuthor_ShouldNotAddCard()
        {
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            var card = service.AddCard(_deck.Id, CardText, CardType);

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
        }

        [Fact]
        public void ExistingDeckId_ShouldReturnNewCard()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var card = service.AddCard(_deck.Id, CardText, CardType);

            card.Should().NotBeNull();
            card!.DeckId.Should().Be(_deck.Id);
            card!.Text.Should().Be(CardText);
            card!.CardType.Should().Be(CardType);
        }

        [Fact]
        public void ExistingDeckId_ShouldAddCard()
        {
            _dbContext.SetupTest(_deck);
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.AddCard(_deck.Id, CardText, CardType);
            var card = _dbContext.Cards.Single();

            _dbContext.Cards.Should().HaveCount(cardCountBefore + 1);
            card.Should().NotBeNull();
            card!.DeckId.Should().Be(_deck.Id);
            card!.Text.Should().Be(CardText);
            card!.CardType.Should().Be(CardType);
        }
    }
}
