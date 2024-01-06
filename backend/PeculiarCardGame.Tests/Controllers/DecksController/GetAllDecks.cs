﻿using FluentAssertions;
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
    public class GetAllDecks
    {
        private readonly IReadOnlyList<Deck> _decks;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public GetAllDecks()
        {
            const int DeckId = 1;
            const int AnotherDeckId = 2;
            const int AuthorId = 1;
            const string Name = "test";
            const string Description = "test";

            var decks = new List<Deck>
            {
                new Deck
                {
                    Id = DeckId,
                    AuthorId = AuthorId,
                    Description = Description,
                    Name = Name
                },
                new Deck
                {
                    Id = AnotherDeckId,
                    AuthorId = AuthorId,
                    Description = Description,
                    Name = Name
                }
            };
            _decks = decks;

            _deckManagementService = Substitute.For<IDeckManagementService>();
            _deckManagementService.GetAllDecks().Returns(decks);

            var authenticationService = Substitute.For<IAuthenticationService>();
            var usersService = Substitute.For<IUsersService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, usersService, _deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task ShouldReturnOkWithDecks()
        {
            var message = await _client.GetAsync("/api/decks");
            var response = await message.Content.ReadFromJsonAsync<List<GetDeckResponse>>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNullOrEmpty();
            response.Should().BeEquivalentTo(_decks.Select(GetDeckResponse.FromDeck));
        }

        [Fact]
        public async Task ShouldCallGetAllDecks()
        {
            await _client.GetAsync("/api/decks");

            _deckManagementService.Received().GetAllDecks();
        }
    }
}
