using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class GetAllCards
    {
        private readonly Deck _deck;
        private readonly Card _card;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public GetAllCards()
        {
            const int CardId = 1;
            const string DeckName = "test";
            const string DeckDescription = "test";
            const int DeckId = 1;
            const string CardText = "test";
            const CardType CardType = CardType.White;

            _deck = new Deck
            {
                Id = DeckId,
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

            _dbContext = TestHelpers.GetDbContext();

            _requestContext = new RequestContext();
        }

        [Fact]
        public void NotExistingDeckId_ShouldReturnErrorTypeNotFound()
        {
            var service = new Service(_dbContext, _requestContext);

            var result = service.GetAllCards(_card.DeckId);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void ShouldReturnAllCards()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(card => card.Id = 0, Enumerable.Repeat(_card, 3).ToArray());
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _requestContext);

            var result = service.GetAllCards(_card.DeckId);

            result.Should().BeRight();
            result.Right.Should().HaveCount(cardCountBefore);
        }

        [Fact]
        public void ShouldNotChangeCardCount()
        {
            _dbContext.SetupTest(card => card.Id = 0, Enumerable.Repeat(_card, 3).ToArray());
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _requestContext);

            service.GetAllCards(_card.DeckId);

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
        }
    }
}
