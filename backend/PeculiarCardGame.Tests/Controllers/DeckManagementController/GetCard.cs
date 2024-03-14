using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.Shared;
using PeculiarCardGame.WebApi.Models.Responses;
using System.Net;
using System.Net.Http.Json;

namespace PeculiarCardGame.UnitTests.Controllers.DeckManagementController
{
    public class GetCard
    {
        private readonly Card _existingCard;
        private readonly Card _notExistingCard;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public GetCard()
        {
            const int ExistingCardId = 1;
            const int NotExistingCardId = 2;
            const int DeckId = 1;
            const string Text = "test";
            const CardType CardType = CardType.Black;

            _existingCard = new Card
            {
                Id = ExistingCardId,
                DeckId = DeckId,
                CardType = CardType,
                Text = Text
            };
            _notExistingCard = new Card
            {
                Id = NotExistingCardId,
                DeckId = DeckId,
                CardType = CardType,
                Text = Text
            };

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetCard(_existingCard.Id).Returns(_existingCard);

            var authenticationService = Substitute.For<IAuthenticationService>();
            var usersService = Substitute.For<IUsersService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, usersService, _deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task NotExistingCardId_ShouldReturnNotFound()
        {
            var message = await _client.GetAsync($"/api/cards/{_notExistingCard.Id}");

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingCardId_ShouldReturnOkWithCard()
        {
            var message = await _client.GetAsync($"/api/cards/{_existingCard.Id}");
            var response = await message.Content.ReadFromJsonAsync<GetCardResponse>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(GetCardResponse.FromCard(_existingCard));
        }

        [Fact]
        public async Task ExistingCardId_ShouldCallGetCard()
        {
            await _client.GetAsync($"/api/cards/{_existingCard.Id}");

            _deckManagementService.Received().GetCard(_existingCard.Id);
        }
    }
}
