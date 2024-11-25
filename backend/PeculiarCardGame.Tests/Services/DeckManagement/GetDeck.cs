using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class GetDeck
    {
        private const int ExistingDeckId = 1;
        private const int NotExistingDeckId = 2;

        private readonly Deck _deck;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public GetDeck()
        {
            const int UserId = 1;
            const string DeckName = "test";
            const string DeckDescription = "test";

            _deck = new Deck
            {
                AuthorId = UserId,
                Id = ExistingDeckId,
                Name = DeckName,
                Description = DeckDescription
            };

            _dbContext = TestHelpers.GetDbContext();

            _requestContext = new RequestContext();
        }

        [Fact]
        public void NotExistingDeckId_ShouldReturnErrorTypeNotFound()
        {
            var service = new Service(_dbContext, _requestContext);

            var result = service.GetDeck(NotExistingDeckId);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void ExistingDeckId_ShouldReturnDeck()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _requestContext);

            var result = service.GetDeck(_deck.Id);

            result.Should().BeRight();
            result.Right.Id.Should().Be(_deck.Id);
            result.Right.AuthorId.Should().Be(_deck.AuthorId);
            result.Right.Name.Should().Be(_deck.Name);
            result.Right.Description.Should().Be(_deck.Description);
        }

        [Theory]
        [InlineData(ExistingDeckId)]
        [InlineData(NotExistingDeckId)]
        public void ShouldNotChangeDeckCount(int deckId)
        {
            _dbContext.SetupTest(_deck);
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _requestContext);

            service.GetDeck(deckId);

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }
    }
}
