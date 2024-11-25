using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.Shared;
using PeculiarCardGame.WebApi.Models.Responses;

namespace PeculiarCardGame.Tests.Controllers.UsersController
{
    public class GetUser
    {
        private readonly User _existingUser;
        private readonly User _notExistingUser;

        private readonly IUsersService _usersService;

        private readonly HttpClient _client;

        public GetUser()
        {
            const int ExistingUserId = 1;
            const int NotExistingUserId = 2;
            const string ExistingUsername = "test";
            const string NotExistingUsername = "notexisting";
            const string DisplayedName = "test";
            const string PasswordHash = "test";

            _existingUser = new User
            {
                Id = ExistingUserId,
                Username = ExistingUsername,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };
            _notExistingUser = new User
            {
                Id = NotExistingUserId,
                Username = NotExistingUsername,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };

            _usersService = Substitute.For<IUsersService>();
            _usersService.GetUser(_existingUser.Id).Returns(_existingUser);
            _usersService.GetUser(_notExistingUser.Id).Returns(ErrorType.NotFound);

            var authenticationService = Substitute.For<IAuthenticationService>();
            var deckManagementService = Substitute.For<IDeckManagementService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, _usersService, deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task NotExistingUserId_ShouldReturnNotFound()
        {
            var message = await _client.GetAsync($"/api/users/{_notExistingUser.Id}");

            message.Should().NotBeNull();
            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingUserId_ShouldReturnUser()
        {
            var message = await _client.GetAsync($"/api/users/{_existingUser.Id}");
            var response = await message.Content.ReadFromJsonAsync<GetUserResponse>();

            message.Should().NotBeNull();
            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(GetUserResponse.FromUser(_existingUser));
        }

        [Fact]
        public async Task ExistingUserId_ShouldUseGetUserMethod()
        {
            await _client.GetAsync($"/api/users/{_existingUser.Id}");

            _usersService.Received().GetUser(_existingUser.Id);
        }
    }
}
