using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class GetAllDecks
    {
        private readonly Deck _deck;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public GetAllDecks()
        {
            const int UserId = 1;
            const string DeckName = "test";
            const string DeckDescription = "test";

            _deck = new Deck
            {
                AuthorId = UserId,
                Name = DeckName,
                Description = DeckDescription
            };

            _dbContext = TestHelpers.GetDbContext();

            _requestContext = new RequestContext();
        }

        [Fact]
        public void ShouldReturnAllDecks()
        {
            _dbContext.SetupTest(deck => deck.Id = 0, Enumerable.Repeat(_deck, 3).ToArray());
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _requestContext);

            var decks = service.GetAllDecks();

            decks.Should().NotBeNull();
            decks.Should().HaveCount(deckCountBefore);
        }

        [Fact]
        public void ShouldNotChangeDeckCount()
        {
            _dbContext.SetupTest(deck => deck.Id = 0, Enumerable.Repeat(_deck, 3).ToArray());
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _requestContext);

            service.GetAllDecks();

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }
    }
}
