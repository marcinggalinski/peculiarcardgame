using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.Shared;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;

namespace PeculiarCardGame.Tests.Controllers.DeckManagementController
{
    public class AddCard
    {
        private readonly Deck _existingDeck;
        private readonly Deck _notExistingDeck;
        private readonly Card _card;

        private readonly IDeckManagementService _deckManagementService;

        private readonly HttpClient _client;

        public AddCard()
        {
            const int ExistingDeckId = 1;
            const int NotExistingDeckId = 2;
            const int CardId = 1;
            const int UserId = 1;
            const string Name = "test";
            const string Description = "test";
            const string Text = "test";
            const string Username = "test";
            const string DisplayedName = "test";
            const string PasswordHash = "test";
            const CardType CardType = CardType.Black;

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
            _card = new Card
            {
                Id = CardId,
                DeckId = ExistingDeckId,
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
            _deckManagementService.GetDeck(_existingDeck.Id).Returns(_existingDeck);
            _deckManagementService.AddCard(_existingDeck.Id, _card.Text, _card.CardType).Returns(_card);
            _deckManagementService.AddCard(_notExistingDeck.Id, Arg.Any<string>(), Arg.Any<CardType>()).Returns(ErrorType.NotFound);

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
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_existingDeck.Id}/cards"),
                Content = JsonContent.Create(new AddCardRequest
                {
                    Text = _card.Text,
                    CardType = _card.CardType
                })
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task NotExistingDeckId_ShouldReturnNotFound()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_notExistingDeck.Id}/cards"),
                Content = JsonContent.Create(new AddCardRequest
                {
                    Text = _card.Text,
                    CardType = _card.CardType
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingDeckId_ShouldReturnCreatedWithAddedCard()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_existingDeck.Id}/cards"),
                Content = JsonContent.Create(new AddCardRequest
                {
                    Text = _card.Text,
                    CardType = _card.CardType
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });
            var response = await message.Content.ReadFromJsonAsync<GetCardResponse>();

            message.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(GetCardResponse.FromCard(_card));
        }

        [Fact]
        public async Task ExistingDeckId_ShouldCallAddCard()
        {
            await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/decks/{_existingDeck.Id}/cards"),
                Content = JsonContent.Create(new AddCardRequest
                {
                    Text = _card.Text,
                    CardType = _card.CardType
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            _deckManagementService.Received().AddCard(_existingDeck.Id, _card.Text, _card.CardType);
        }
    }
}
