using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.UnitTests.Services.DeckManagement
{
    public class GetDeck
    {
        const int ExistingDeckId = 1;
        const int NotExistingDeckId = 2;

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
        public void NotExistingDeckId_ShouldReturnNull()
        {
            var service = new Service(_dbContext, _requestContext);

            var deck = service.GetDeck(NotExistingDeckId);

            deck.Should().BeNull();
        }

        [Fact]
        public void ExistingDeckId_ShouldReturnDeck()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _requestContext);

            var deck = service.GetDeck(_deck.Id);

            deck.Should().NotBeNull();
            deck!.Id.Should().Be(_deck.Id);
            deck!.AuthorId.Should().Be(_deck.AuthorId);
            deck!.Name.Should().Be(_deck.Name);
            deck!.Description.Should().Be(_deck.Description);
        }

        [Theory]
        [InlineData(ExistingDeckId)]
        [InlineData(NotExistingDeckId)]
        public void ShouldNotChangeDeckCount(int deckId)
        {
            _dbContext.SetupTest(_deck);
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _requestContext);

            var deck = service.GetDeck(deckId);

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }
    }
}
