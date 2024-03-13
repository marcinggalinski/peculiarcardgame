using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.Shared;
using System.Net;

namespace PeculiarCardGame.UnitTests.Controllers.CardsController
{
    public class DeleteCard
    {
        private readonly Card _existingCard;
        private readonly Card _notExistingCard;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public DeleteCard()
        {
            const int ExistingCardId = 1;
            const int NotExistingCardId = 2;
            const int DeckId = 1;
            const int UserId = 1;
            const string Text = "test";
            const string Username = "test";
            const string DisplayedName = "test";
            const string PasswordHash = "test";
            const CardType CardType = CardType.White;

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

            var user = new User
            {
                Id = UserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetCard(_existingCard.Id).Returns(_existingCard);
            _deckManagementService.DeleteCard(Arg.Any<int>()).Returns(false);
            _deckManagementService.DeleteCard(_existingCard.Id).Returns(true);

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
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/cards/{_existingCard.Id}")
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task NotExistingCardId_ShouldReturnNotFound()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/cards/{_notExistingCard.Id}"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingCardId_ShouldReturnOk()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/cards/{_existingCard.Id}"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ExistingCardId_ShouldCallDeleteCard()
        {
            await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/cards/{_existingCard.Id}"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            _deckManagementService.Received().DeleteCard(_existingCard.Id);
        }
    }
}
