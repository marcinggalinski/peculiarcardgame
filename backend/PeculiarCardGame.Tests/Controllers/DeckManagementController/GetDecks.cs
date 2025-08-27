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
    public class GetDecks
    {
        private const int UserId = 1;
        private const string MatchingQuery = "matching";
        private const string NotMatchingQuery = "not matching";

        private readonly IReadOnlyList<Deck> _decks;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public GetDecks()
        {
            const int DeckId = 1;
            const int AnotherDeckId = 2;
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

            var decks = new List<Deck>
            {
                new Deck
                {
                    Id = DeckId,
                    AuthorId = user.Id,
                    Author = user,
                    Description = Description,
                    Name = Name
                },
                new Deck
                {
                    Id = AnotherDeckId,
                    AuthorId = user.Id,
                    Author = user,
                    Description = Description,
                    Name = Name
                }
            };
            _decks = decks;

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.SearchDecks(Arg.Any<string?>(), Arg.Any<int?>()).Returns([]);
            _deckManagementService.SearchDecks(Arg.Is<string?>(x => string.IsNullOrEmpty(x)), Arg.Any<int?>()).Returns(decks);
            _deckManagementService.SearchDecks(MatchingQuery).Returns(decks);

            var authenticationService = Substitute.For<IAuthenticationService>();
            var usersService = Substitute.For<IUsersService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, usersService, _deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task NoQuery_ShouldReturnOkWithDecks()
        {
            var message = await _client.GetAsync("/api/decks");
            var response = await message.Content.ReadFromJsonAsync<List<GetDeckResponse>>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNullOrEmpty();
            response.Should().BeEquivalentTo(_decks.Select(GetDeckResponse.FromDeck));
        }

        [Fact]
        public async Task MatchingQuery_ShouldReturnOkWithFoundDecks()
        {
            var message = await _client.GetAsync($"/api/decks?filter={MatchingQuery}");
            var response = await message.Content.ReadFromJsonAsync<List<GetDeckResponse>>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNullOrEmpty();
            response.Should().BeEquivalentTo(_decks.Select(GetDeckResponse.FromDeck));
        }

        [Fact]
        public async Task NotMatchingQuery_ShouldReturnOkWithEmptyList()
        {
            var message = await _client.GetAsync($"/api/decks?filter={NotMatchingQuery}");
            var response = await message.Content.ReadFromJsonAsync<List<GetDeckResponse>>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null, UserId)]
        [InlineData(MatchingQuery, null)]
        [InlineData(NotMatchingQuery, null)]
        public async Task ShouldCallSearchDecks(string? filter, int? authorId)
        {
            await _client.GetAsync($"/api/decks?filter={filter}&authorId={authorId}");

            _deckManagementService.Received().SearchDecks(filter, authorId);
        }
    }
}
