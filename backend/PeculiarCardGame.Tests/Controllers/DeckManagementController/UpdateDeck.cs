using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;

namespace PeculiarCardGame.Tests.Controllers.DeckManagementController
{
    public class UpdateDeck
    {
        private readonly Deck _existingDeck;
        private readonly Deck _notExistingDeck;
        private readonly Deck _updatedDeck;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public UpdateDeck()
        {
            const int ExistingDeckId = 1;
            const int NotExistingDeckId = 2;
            const int UserId = 1;
            const string Name = "test";
            const string Description = "test";
            const string UpdatedName = "updated";
            const string UpdatedDescription = "updated";
            const string Username = "test";
            const string DisplayedName = "test";
            const string PasswordHash = "test";

            var user = new User
            {
                Id = UserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };

            _existingDeck = new Deck()
            {
                Id = ExistingDeckId,
                AuthorId = user.Id,
                Author = user,
                Name = Name,
                Description = Description
            };
            _notExistingDeck = new Deck
            {
                Id = NotExistingDeckId,
                AuthorId = user.Id,
                Author = user,
                Name = Name,
                Description = Description
            };
            _updatedDeck = new Deck()
            {
                Id = ExistingDeckId,
                AuthorId = user.Id,
                Author = user,
                Name = UpdatedName,
                Description = UpdatedDescription
            };

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetDeck(_existingDeck.Id).Returns(_existingDeck);
            _deckManagementService.UpdateDeck(_existingDeck.Id, Arg.Any<string?>(), Arg.Any<string?>()).Returns(_updatedDeck);

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
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_existingDeck.Id}"),
                Content = JsonContent.Create(new UpdateDeckRequest
                {
                    NameUpdate = _updatedDeck.Name,
                    DescriptionUpdate = _updatedDeck.Description
                })
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task NotExistingDeckId_ShouldReturnNotFound()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_notExistingDeck.Id}"),
                Content = JsonContent.Create(new UpdateDeckRequest
                {
                    NameUpdate = _updatedDeck.Name,
                    DescriptionUpdate = _updatedDeck.Description
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingDeckId_ShouldReturnOkWithUpdatedDeck()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_existingDeck.Id}"),
                Content = JsonContent.Create(new UpdateDeckRequest
                {
                    NameUpdate = _updatedDeck.Name,
                    DescriptionUpdate = _updatedDeck.Description
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });
            var response = await message.Content.ReadFromJsonAsync<GetDeckResponse>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(GetDeckResponse.FromDeck(_updatedDeck));
        }

        [Fact]
        public async Task ExistingDeckId_ShouldCallUpdateDeck()
        {
            await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_existingDeck.Id}"),
                Content = JsonContent.Create(new UpdateDeckRequest
                {
                    NameUpdate = _updatedDeck.Name,
                    DescriptionUpdate = _updatedDeck.Description
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            _deckManagementService.Received().UpdateDeck(_existingDeck.Id, _updatedDeck.Name, _updatedDeck.Description);
        }
    }
}
