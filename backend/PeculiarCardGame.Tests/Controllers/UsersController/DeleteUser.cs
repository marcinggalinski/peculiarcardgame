using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using System.Net;
using System.Net.Http.Json;

namespace PeculiarCardGame.UnitTests.Controllers.UsersController
{
    public class DeleteUser
    {
        private readonly User _existingUser;
        private readonly User _notExistingUser;

        private readonly IUsersService _usersService;

        private readonly HttpClient _client;

        public DeleteUser()
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
            _usersService.GetUser(_notExistingUser.Id).Returns((User?)null);
            _usersService.DeleteUser(_existingUser.Id).Returns(true);
            _usersService.DeleteUser(_notExistingUser.Id).Returns(false);

            var authenticationService = Substitute.For<IAuthenticationService>();
            authenticationService.Authenticate(Arg.Any<string>()).Returns(_existingUser);

            var deckManagementService = Substitute.For<IDeckManagementService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, _usersService, deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task NoAuthorizationHeader_ShouldReturnUnauthorized()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/users/{_existingUser.Id}")
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task NotExistingUserId_ShouldReturnNotFound()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/users/{_notExistingUser.Id}"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingUserId_ShouldCallDeleteUser()
        {
            await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/users/{_existingUser.Id}"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            _usersService.Received().DeleteUser(_existingUser.Id);
        }

        [Fact]

        public async Task ExistingUserId_ShouldReturnOk()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/users/{_existingUser.Id}"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
