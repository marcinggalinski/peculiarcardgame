using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class DeleteCard
    {
        private readonly Deck _deck;
        private readonly Card _card;
        private readonly Card _anotherCard;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _authorFilledRequestContext;
        private readonly RequestContext _notAuthorFilledRequestContext;

        public DeleteCard()
        {
            const int DeckId = 1;
            const int AuthorId = 1;
            const int NotAuthorId = 2;
            const int CardId = 1;
            const int AnotherCardId = 2;
            const string DeckName = "test";
            const string DeckDescription = "test";
            const string AuthorUsername = "test";
            const string DisplayedName = "test";
            const string NotAuthorUsername = "test";
            const string CardText = "test";
            const CardType CardType = CardType.Black;

            _deck = new Deck
            {
                Id = DeckId,
                AuthorId = AuthorId,
                Name = DeckName,
                Description = DeckDescription
            };
            _card = new Card
            {
                Id = CardId,
                DeckId = DeckId,
                Text = CardText,
                CardType = CardType,
            };
            _anotherCard = new Card
            {
                Id = AnotherCardId,
                DeckId = DeckId,
                Text = CardText,
                CardType = CardType,
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
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _emptyRequestContext);

            var action = () => service.DeleteCard(_card.Id);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldNotDeleteCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
                service.DeleteCard(_card.Id);
            }
            catch (InvalidOperationException) { }
            var card = _dbContext.Cards.SingleOrDefault(x => x.Id == _card.Id);

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
            card.Should().NotBeNull();
        }

        [Fact]
        public void NotExistingCardId_ShouldReturnErrorTypeNotFound()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var error = service.DeleteCard(_anotherCard.Id);

            error.Should().NotBeNull();
            error.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void NotExistingCardId_ShouldNotDeleteCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.DeleteCard(_anotherCard.Id);
            var card = _dbContext.Cards.SingleOrDefault(x => x.Id == _card.Id);

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
            card.Should().NotBeNull();
        }

        [Fact]
        public void NotAuthor_ShouldReturnErrorTypeUnauthorized()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            var error = service.DeleteCard(_card.Id);

            error.Should().NotBeNull();
            error.Should().Be(ErrorType.Unauthorized);
        }

        [Fact]
        public void NotAuthor_ShouldNotDeleteCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            service.DeleteCard(_card.Id);
            var card = _dbContext.Cards.SingleOrDefault(x => x.Id == _card.Id);

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
            card.Should().NotBeNull();
        }

        [Fact]
        public void ExistingCardId_ShouldReturnNull()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var error = service.DeleteCard(_card.Id);

            error.Should().BeNull();
        }

        [Fact]
        public void ExistingCardId_ShouldDeleteCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.DeleteCard(_card.Id);
            var card = _dbContext.Cards.SingleOrDefault(x => x.Id == _card.Id);

            _dbContext.Cards.Should().HaveCount(cardCountBefore - 1);
            card.Should().BeNull();
        }
    }
}
