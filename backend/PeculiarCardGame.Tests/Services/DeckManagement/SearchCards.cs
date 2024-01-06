using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.UnitTests.Services.DeckManagement
{
    public class SearchCards
    {
        private const string Query = "arch";

        private const int SearchedCardCount = 2;

        private readonly Deck _deck;
        private readonly Card _searchedCard;
        private readonly Card _notSearchedCard;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public SearchCards()
        {
            const int UserId = 1;
            const int DeckId = 1;
            const string DeckName = "test";
            const string DeckDescription = "test";
            const int SearchedCardId = 1;
            const int NotSearchedCardId = 2;
            const string SearchedCardText = "searched";
            const string NotSearchedCardText = "test";
            const CardType CardType = CardType.White;

            _deck = new Deck
            {
                Id = DeckId,
                AuthorId = UserId,
                Name = DeckName,
                Description = DeckDescription
            };
            _searchedCard = new Card
            {
                Id = SearchedCardId,
                DeckId = DeckId,
                Text = SearchedCardText,
                CardType = CardType
            };
            _notSearchedCard = new Card
            {
                Id = NotSearchedCardId,
                DeckId = DeckId,
                Text = NotSearchedCardText,
                CardType = CardType
            };

            _dbContext = TestHelpers.GetDbContext();

            _requestContext = new RequestContext();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void NullOrEmptyQuery_ShouldReturnAllCards(string? query)
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(card => card.Id = 0, Enumerable.Repeat(_searchedCard, SearchedCardCount).ToArray());
            var service = new Service(_dbContext, _requestContext);

            var cards = service.SearchCards(_deck.Id, query);

            cards.Should().HaveCount(SearchedCardCount);
        }

        [Fact]
        public void NotEmptyQuery_ShouldReturnAllMatchingCards()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(card => card.Id = 0, Enumerable.Repeat(_notSearchedCard, 2).ToArray());
            _dbContext.SetupTest(card => card.Id = 0, Enumerable.Repeat(_searchedCard, SearchedCardCount).ToArray());
            var service = new Service(_dbContext, _requestContext);

            var cards = service.SearchCards(_deck.Id, Query);

            cards.Should().HaveCount(SearchedCardCount);
        }

        [Fact]
        public void NotExistingDeckId_ShouldReturnNull()
        {
            var service = new Service(_dbContext, _requestContext);

            var cards = service.SearchCards(_deck.Id, Query);

            cards.Should().BeNull();
        }

        [Fact]
        public void ShouldNotChangeDeckCount()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(card => card.Id = 0, Enumerable.Repeat(_searchedCard, SearchedCardCount).ToArray());
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _requestContext);

            service.SearchCards(_deck.Id, Query);

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
        }
    }
}
