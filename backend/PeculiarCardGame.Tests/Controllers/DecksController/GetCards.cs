using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.WebApi.Models.Responses;
using System.Net;
using System.Net.Http.Json;

namespace PeculiarCardGame.UnitTests.Controllers.DecksController
{
    public class GetCards
    {
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
            const string Username = "test";
            const string DisplayedName = "test";
            const string PasswordHash = "test";
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

            var user = new User
            {
                Id = UserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetDeck(_existingDeck.Id).Returns(_existingDeck);
            _deckManagementService.GetAllCards(_existingDeck.Id).Returns(cards);

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
        public async Task ExistingDeckId_ShouldReturnOkWithCards()
        {
            var message = await _client.GetAsync($"/api/decks/{_existingDeck.Id}/cards");
            var response = await message.Content.ReadFromJsonAsync<List<GetCardResponse>>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNullOrEmpty();
            response.Should().BeEquivalentTo(_cards.Select(GetCardResponse.FromCard));
        }

        [Fact]
        public async Task ExistingDeckId_ShouldCallGetAllCards()
        {
            await _client.GetAsync($"/api/decks/{_existingDeck.Id}/cards");

            _deckManagementService.Received().GetAllCards(_existingDeck.Id);
        }
    }
}
