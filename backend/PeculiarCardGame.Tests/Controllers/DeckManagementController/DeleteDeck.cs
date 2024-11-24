using System.Net;
using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;

namespace PeculiarCardGame.Tests.Controllers.DeckManagementController
{
    public class DeleteDeck
    {
        private readonly Deck _existingDeck;
        private readonly Deck _notExistingDeck;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public DeleteDeck()
        {
            const int ExistingDeckId = 1;
            const int NotExistingDeckId = 2;
            const int UserId = 1;
            const string Name = "test";
            const string Description = "test";
            const string Username = "test";
            const string DisplayedName = "test";
            const string PasswordHash = "test";

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

            var user = new User
            {
                Id = UserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetDeck(_existingDeck.Id).Returns(_existingDeck);
            _deckManagementService.DeleteDeck(_existingDeck.Id).Returns(true);
            _deckManagementService.DeleteDeck(_notExistingDeck.Id).Returns(false);

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
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_existingDeck.Id}")
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task NotExistingDeckId_ShouldReturnNotFound()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_notExistingDeck.Id}"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingDeckId_ShouldReturnOk()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_existingDeck.Id}"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ExistingDeckId_ShouldCallDeleteDeck()
        {
            await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_existingDeck.Id}"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            _deckManagementService.Received().DeleteDeck(_existingDeck.Id);
        }
    }
}
