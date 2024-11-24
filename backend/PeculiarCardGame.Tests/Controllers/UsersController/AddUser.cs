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
    public class AddUser
    {
        private const string Password = "test";

        private readonly User _existingUser;
        private readonly User _notExistingUser;

        private readonly IUsersService _usersService;

        private readonly HttpClient _client;

        public AddUser()
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
            _usersService.AddUser(_notExistingUser.Username, Arg.Any<string>(), Arg.Any<string>()).Returns(_notExistingUser);

            var authenticationService = Substitute.For<IAuthenticationService>();
            authenticationService.Authenticate(Arg.Any<string>()).Returns(_existingUser);

            var deckManagementService = Substitute.For<IDeckManagementService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, _usersService, deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task FilledRequestContext_ShouldReturnUnprocessableEntityAsync()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "/api/users"),
                Content = JsonContent.Create(new AddUserRequest
                {
                    Username = _notExistingUser.Username,
                    DisplayedName = _notExistingUser.DisplayedName,
                    Password = Password
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer token" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task UsernameInUse_ShouldReturnConflict()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "/api/users"),
                Content = JsonContent.Create(new AddUserRequest
                {
                    Username = _existingUser.Username,
                    DisplayedName = _existingUser.DisplayedName,
                    Password = Password
                })
            });

            message.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task NoSuchUser_ShouldReturnCreatedWithNewUser()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "/api/users"),
                Content = JsonContent.Create(new AddUserRequest
                {
                    Username = _notExistingUser.Username,
                    DisplayedName = _notExistingUser.DisplayedName,
                    Password = Password
                })
            });
            var response = await message.Content.ReadFromJsonAsync<GetUserResponse>();

            message.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(GetUserResponse.FromUser(_notExistingUser));
        }

        [Fact]
        public async Task NoSuchUser_ShouldAddUser()
        {
            await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "/api/users"),
                Content = JsonContent.Create(new AddUserRequest
                {
                    Username = _notExistingUser.Username,
                    DisplayedName = _notExistingUser.DisplayedName,
                    Password = Password
                })
            });

            _usersService.Received().AddUser(_notExistingUser.Username, _notExistingUser.DisplayedName, Password);
        }
    }
}
