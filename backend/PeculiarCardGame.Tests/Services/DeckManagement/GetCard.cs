using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class GetCard
    {
        private const int ExistingCardId = 1;
        private const int NotExistingCardId = 2;

        private readonly Deck _deck;
        private readonly Card _card;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public GetCard()
        {
            const int DeckId = 1;
            const string DeckName = "test";
            const string DeckDescription = "test";
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
                Id = ExistingCardId,
                DeckId = DeckId,
                Text = CardText,
                CardType = CardType,
            };

            _dbContext = TestHelpers.GetDbContext();

            _requestContext = new RequestContext();
        }

        [Fact]
        public void NotExistingCardId_ShouldReturnErrorTypeNotFound()
        {
            var service = new Service(_dbContext, _requestContext);

            var result = service.GetCard(_card.Id);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void ExistingCardId_ShouldReturnCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _requestContext);

            var result = service.GetCard(_card.Id);

            result.Should().BeRight();
            result.Right.Id.Should().Be(_card.Id);
            result.Right.DeckId.Should().Be(_card.DeckId);
            result.Right.Text.Should().Be(_card.Text);
            result.Right.CardType.Should().Be(_card.CardType);
        }

        [Theory]
        [InlineData(ExistingCardId)]
        [InlineData(NotExistingCardId)]
        public void ShouldNotChangeCardCount(int cardId)
        {
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _requestContext);

            service.GetCard(cardId);

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
        }
    }
}
