using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.Shared;
using PeculiarCardGame.WebApi.Models.Responses;

namespace PeculiarCardGame.Tests.Controllers.DeckManagementController
{
    public class GetCards
    {
        private const string MatchingQuery = "matching";
        private const string NotMatchingQuery = "not matching";

        private readonly Deck _existingDeck;
        private readonly Deck _notExistingDeck;
        private readonly IReadOnlyList<Card> _cards;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public GetCards()
        {
            const int ExistingDeckId = 1;
            const int NotExistingDeckId = 2;
            const int CardId = 1;
            const int AnotherCardId = 2;
            const int UserId = 1;
            const string Name = "test";
            const string Description = "test";
            const string Text = "test";
            const CardType CardType = CardType.White;

            _existingDeck = new Deck
            {
                Id = ExistingDeckId,
                AuthorId = UserId,
                Name = Name,
                Description = Description
            };
            _notExistingDeck = new Deck
            {
                Id = NotExistingDeckId,
                AuthorId = UserId,
                Name = Name,
                Description = Description
            };

            var cards = new List<Card>
            {
                new Card
                {
                    Id = CardId,
                    DeckId = ExistingDeckId,
                    CardType = CardType,
                    Text = Text
                },
                new Card
                {
                    Id = AnotherCardId,
                    DeckId = ExistingDeckId,
                    CardType = CardType,
                    Text = Text
                }
            };
            _cards = cards;

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetDeck(_existingDeck.Id).Returns(_existingDeck);
            _deckManagementService.GetDeck(_notExistingDeck.Id).Returns(ErrorType.NotFound);
            _deckManagementService.SearchCards(_existingDeck.Id, Arg.Any<string?>()).Returns(new List<Card>());
            _deckManagementService.SearchCards(_existingDeck.Id, MatchingQuery).Returns(cards);
            _deckManagementService.SearchCards(Arg.Any<int>(), Arg.Is<string?>(x => string.IsNullOrEmpty(x))).Returns(cards);
            _deckManagementService.SearchCards(_notExistingDeck.Id, Arg.Any<string?>()).Returns(ErrorType.NotFound);

            var authenticationService = Substitute.For<IAuthenticationService>();
            var usersService = Substitute.For<IUsersService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, usersService, _deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task NotExistingDeckId_ShouldReturnNotFound()
        {
            var message = await _client.GetAsync($"/api/decks/{_notExistingDeck.Id}/cards");

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task NoQuery_ShouldReturnOkWithCards()
        {
            var message = await _client.GetAsync($"/api/decks/{_existingDeck.Id}/cards");
            var response = await message.Content.ReadFromJsonAsync<List<GetCardResponse>>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNullOrEmpty();
            response.Should().BeEquivalentTo(_cards.Select(GetCardResponse.FromCard));
        }

        [Fact]
        public async Task NotMatchingQuery_ShouldReturnOkWithEmptyList()
        {
            var message = await _client.GetAsync($"/api/decks/{_existingDeck.Id}/cards?filter={NotMatchingQuery}");
            var response = await message.Content.ReadFromJsonAsync<List<GetCardResponse>>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }

        [Fact]
        public async Task MatchingQuery_ShouldReturnOkWithCards()
        {
            var message = await _client.GetAsync($"/api/decks/{_existingDeck.Id}/cards?filter={MatchingQuery}");
            var response = await message.Content.ReadFromJsonAsync<List<GetCardResponse>>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNullOrEmpty();
            response.Should().BeEquivalentTo(_cards.Select(GetCardResponse.FromCard));
        }

        [Theory]
        [InlineData(MatchingQuery)]
        [InlineData(NotMatchingQuery)]
        public async Task ShouldCallSearchCards(string query)
        {
            await _client.GetAsync($"/api/decks/{_existingDeck.Id}/cards?filter={query}");

            _deckManagementService.Received().SearchCards(_existingDeck.Id, query);
        }
    }
}
