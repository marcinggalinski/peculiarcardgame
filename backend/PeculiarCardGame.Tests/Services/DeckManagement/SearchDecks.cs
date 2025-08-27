using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class SearchDecks
    {
        private const int UserId = 1;
        private const int AnotherUserId = 2;
        private const string Query = "arch";

        private readonly Deck _deck;
        private readonly Deck _searchedNameDeck;
        private readonly Deck _searchedDescriptionDeck;
        private readonly Deck _anotherUsersDeck;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public SearchDecks()
        {
            const string DeckName = "test";
            const string DeckDescription = "test";
            const string SearchedName = "searched";
            const string SearchedDescription = "searched";

            _deck = new Deck
            {
                AuthorId = UserId,
                Name = DeckName,
                Description = DeckDescription
            };
            _searchedNameDeck = new Deck
            {
                AuthorId = UserId,
                Name = SearchedName,
                Description = DeckDescription
            };
            _searchedDescriptionDeck = new Deck
            {
                AuthorId = UserId,
                Name = DeckName,
                Description = SearchedDescription
            };
            _anotherUsersDeck = new Deck
            {
                AuthorId = AnotherUserId,
                Name = DeckName,
                Description = DeckDescription
            };

            _dbContext = TestHelpers.GetDbContext();

            _requestContext = new RequestContext();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void NullOrEmptyQuery_ShouldReturnAllDecks(string? query)
        {
            _dbContext.SetupTest(deck => deck.Id = 0, Enumerable.Repeat(_deck, 3).ToArray());
            var deckCount = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _requestContext);

            var decks = service.SearchDecks(query);

            decks.Should().NotBeNull();
            decks.Should().HaveCount(deckCount);
        }

        [Fact]
        public void NotEmptyQuery_ShouldReturnAllMatchingDecks()
        {
            const int SearchedNameDeckCount = 2;
            const int SearchedDescriptionDeckCount = 2;
            
            _dbContext.SetupTest(deck => deck.Id = 0, Enumerable.Repeat(_deck, 3).ToArray());
            _dbContext.SetupTest(deck => deck.Id = 0, Enumerable.Repeat(_searchedNameDeck, SearchedNameDeckCount).ToArray());
            _dbContext.SetupTest(deck => deck.Id = 0, Enumerable.Repeat(_searchedDescriptionDeck, SearchedDescriptionDeckCount).ToArray());
            var service = new Service(_dbContext, _requestContext);

            var decks = service.SearchDecks(Query);

            decks.Should().NotBeNull();
            decks.Should().HaveCount(SearchedNameDeckCount + SearchedDescriptionDeckCount);
            decks.Should().AllSatisfy(x =>
                x.Should().Match<Deck>(y => y.Name.Contains(Query) || y.Description.Contains(Query)));
        }

        [Fact]
        public void ShouldNotChangeDeckCount()
        {
            _dbContext.SetupTest(deck => deck.Id = 0, Enumerable.Repeat(_deck, 3).ToArray());
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _requestContext);

            service.SearchDecks(Query);

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }

        [Fact]
        public void NotEmptyAuthorId_ShouldFilterDecks()
        {
            const int UsersDeckCount = 3;
            
            _dbContext.SetupTest(deck => deck.Id = 0, Enumerable.Repeat(_deck, UsersDeckCount).ToArray());
            _dbContext.SetupTest(deck => deck.Id = 0, Enumerable.Repeat(_anotherUsersDeck, 3).ToArray());
            var service = new Service(_dbContext, _requestContext);

            var decks = service.SearchDecks("", UserId);
            
            decks.Should().NotBeNull();
            decks.Should().HaveCount(UsersDeckCount);
            decks.Should().OnlyContain(x => x.AuthorId == UserId);
        }
    }
}
