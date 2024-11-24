using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.WebApi.Models.Responses;

namespace PeculiarCardGame.Tests.Controllers.DeckManagementController
{
    public class GetDeck
    {
        private readonly Deck _existingDeck;
        private readonly Deck _notExistingDeck;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public GetDeck()
        {
            const int ExistingDeckId = 1;
            const int NotExistingDeckId = 2;
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

            _existingDeck = new Deck
            {
                Id = ExistingDeckId,
                AuthorId = user.Id,
                Author = user,
                Description = Description,
                Name = Name
            };
            _notExistingDeck = new Deck
            {
                Id = NotExistingDeckId,
                Description = Description,
                Name = Name
            };

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetDeck(_existingDeck.Id).Returns(_existingDeck);

            var authenticationService = Substitute.For<IAuthenticationService>();
            var usersService = Substitute.For<IUsersService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, usersService, _deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task NotExistingDeckId_ShouldReturnNotFound()
        {
            var message = await _client.GetAsync($"/api/decks/{_notExistingDeck.Id}");

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingDeckId_ShouldReturnOkWithDeck()
        {
            var message = await _client.GetAsync($"/api/decks/{_existingDeck.Id}");
            var response = await message.Content.ReadFromJsonAsync<GetDeckResponse>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(GetDeckResponse.FromDeck(_existingDeck));
        }

        [Fact]
        public async Task ExistingDeckId_ShouldCallGetUser()
        {
            await _client.GetAsync($"/api/decks/{_existingDeck.Id}");

            _deckManagementService.Received().GetDeck(_existingDeck.Id);
        }
    }
}
