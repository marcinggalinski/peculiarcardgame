using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.Shared;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;
using System.Net;
using System.Net.Http.Json;

namespace PeculiarCardGame.UnitTests.Controllers.CardsController
{
    public class UpdateCard
    {
        private readonly Card _existingCard;
        private readonly Card _notExistingCard;
        private readonly Card _updatedCard;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public UpdateCard()
        {
            const int ExistingCardId = 1;
            const int NotExistingCardId = 2;
            const int DeckId = 1;
            const int UserId = 1;
            const string Text = "test";
            const string Username = "test";
            const string DisplayedName = "test";
            const string PasswordHash = "test";
            const string TextUpdate = "updated";
            const CardType CardType = CardType.White;
            const CardType CardTypeUpdate = CardType.Black;

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
            _updatedCard = new Card
            {
                Id = ExistingCardId,
                DeckId = DeckId,
                CardType = CardTypeUpdate,
                Text = TextUpdate
            };

            var user = new User
            {
                Id = UserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetCard(_existingCard.Id).Returns(_existingCard);
            _deckManagementService.UpdateCard(_existingCard.Id, TextUpdate, CardTypeUpdate).Returns(_updatedCard);

            var authenticationService = Substitute.For<IAuthenticationService>();
            authenticationService.Authenticate(Arg.Any<string>()).Returns(user);

            var usersService = Substitute.For<IUsersService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, usersService, _deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task NoAuthorizationHeader_ShouldReturnUnauthorized()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/cards/{_existingCard.Id}"),
                Content = JsonContent.Create(new UpdateCardRequest
                {
                    CardTypeUpdate = _updatedCard.CardType,
                    TextUpdate = _updatedCard.Text
                })
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task NotExistingCardId_ShouldReturnNotFound()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/cards/{_notExistingCard.Id}"),
                Content = JsonContent.Create(new UpdateCardRequest
                {
                    CardTypeUpdate = _updatedCard.CardType,
                    TextUpdate = _updatedCard.Text
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingCardId_ShouldReturnOkWithUpdatedCard()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/cards/{_existingCard.Id}"),
                Content = JsonContent.Create(new UpdateCardRequest
                {
                    CardTypeUpdate = _updatedCard.CardType,
                    TextUpdate = _updatedCard.Text
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });
            var response = await message.Content.ReadFromJsonAsync<GetCardResponse>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(GetCardResponse.FromCard(_updatedCard));
        }

        [Fact]
        public async Task ExistingCardId_ShouldCallUpdateCard()
        {
            await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/cards/{_existingCard.Id}"),
                Content = JsonContent.Create(new UpdateCardRequest
                {
                    CardTypeUpdate = _updatedCard.CardType,
                    TextUpdate = _updatedCard.Text
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            _deckManagementService.Received().UpdateCard(_existingCard.Id, _updatedCard.Text, _updatedCard.CardType);
        }
    }
}
