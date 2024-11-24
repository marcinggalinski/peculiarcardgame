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
    public class AddDeck
    {
        private readonly Deck _deck;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public AddDeck()
        {
            const int DeckId = 1;
            const int UserId = 1;
            const string Name = "test";
            const string Description = "test";
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

            _deck = new Deck
            {
                Id = DeckId,
                AuthorId = user.Id,
                Author = user,
                Description = Description,
                Name = Name
            };

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetDeck(_deck.Id).Returns(_deck);
            _deckManagementService.AddDeck(_deck.Name, _deck.Description).Returns(_deck);

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
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "/api/decks"),
                Content = JsonContent.Create(new AddDeckRequest
                {
                    Name = _deck.Name,
                    Description = _deck.Description
                })
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task NullName_ShouldReturnBadRequest()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "/api/decks"),
                Content = JsonContent.Create(new AddDeckRequest
                {
                    Description = _deck.Description
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task EverythingOk_ShouldReturnCreatedWithDeck()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "/api/decks"),
                Content = JsonContent.Create(new AddDeckRequest
                {
                    Name = _deck.Name,
                    Description = _deck.Description
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });
            var response = await message.Content.ReadFromJsonAsync<GetDeckResponse>();

            message.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(GetDeckResponse.FromDeck(_deck));
        }

        [Fact]
        public async Task EverythingOk_ShouldCallAddDeck()
        {
            await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "/api/decks"),
                Content = JsonContent.Create(new AddDeckRequest
                {
                    Name = _deck.Name,
                    Description = _deck.Description
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            _deckManagementService.Received().AddDeck(_deck.Name, _deck.Description);
        }
    }
}
