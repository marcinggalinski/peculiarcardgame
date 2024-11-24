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

namespace PeculiarCardGame.Tests.Controllers.UsersController
{
    public class UpdateUser
    {
        private const string DisplayedNameUpdate = "updated";
        private const string PasswordUpdate = "updated";

        private readonly User _existingUser;
        private readonly User _notExistingUser;
        private readonly User _updatedUser;

        private readonly IUsersService _usersService;

        private readonly HttpClient _client;

        public UpdateUser()
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
            _updatedUser = new User
            {
                Id = ExistingUserId,
                Username = ExistingUsername,
                DisplayedName = DisplayedNameUpdate,
                PasswordHash = PasswordUpdate
            };

            _usersService = Substitute.For<IUsersService>();
            _usersService.GetUser(_existingUser.Id).Returns(_existingUser);
            _usersService.UpdateUser(_existingUser.Id, DisplayedNameUpdate, PasswordUpdate).Returns(_updatedUser);

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
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/users/{_existingUser.Id}"),
                Content = JsonContent.Create(new UpdateUserRequest
                {
                    DisplayedNameUpdate = DisplayedNameUpdate,
                    PasswordUpdate = PasswordUpdate
                })
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task NotExistingUserId_ShouldReturnNotFound()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/users/{_notExistingUser.Id}"),
                Content = JsonContent.Create(new UpdateUserRequest
                {
                    DisplayedNameUpdate = DisplayedNameUpdate,
                    PasswordUpdate = PasswordUpdate
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingUserId_ShouldCallUpdateUser()
        {
            await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/users/{_existingUser.Id}"),
                Content = JsonContent.Create(new UpdateUserRequest
                {
                    DisplayedNameUpdate = DisplayedNameUpdate,
                    PasswordUpdate = PasswordUpdate
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            _usersService.Received().UpdateUser(_existingUser.Id, DisplayedNameUpdate, PasswordUpdate);
        }

        [Fact]
        public async Task ExistingUserId_ShouldReturnOkWithUpdatedUser()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(_client.BaseAddress!, $"/api/users/{_existingUser.Id}"),
                Content = JsonContent.Create(new UpdateUserRequest
                {
                    DisplayedNameUpdate = DisplayedNameUpdate,
                    PasswordUpdate = PasswordUpdate
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });
            var response = await message.Content.ReadFromJsonAsync<GetUserResponse>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(GetUserResponse.FromUser(_updatedUser));
        }
    }
}
