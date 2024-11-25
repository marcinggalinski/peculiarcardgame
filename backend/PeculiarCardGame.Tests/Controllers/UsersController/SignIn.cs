using System.Net;
using System.Net.Http.Json;
using System.Text;
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
    public class SignIn
    {
        private const string BearerToken = "token";

        private readonly User _existingUser;
        private readonly User _notExistingUser;

        private readonly HttpClient _client;

        public SignIn()
        {
            const int ExistingUserId = 1;
            const int NotExistingUserId = 2;
            const string ExistingUsername = "test";
            const string NotExistingUsername = "notexisting";
            const string DisplayedName = "test";
            const string ValidPasswordHash = "test";
            const string InvalidPasswordHash = "invalid";

            _existingUser = new User
            {
                Id = ExistingUserId,
                Username = ExistingUsername,
                DisplayedName = DisplayedName,
                PasswordHash = ValidPasswordHash
            };
            _notExistingUser = new User
            {
                Id = NotExistingUserId,
                Username = NotExistingUsername,
                DisplayedName = DisplayedName,
                PasswordHash = InvalidPasswordHash
            };

            var authenticationService = Substitute.For<IAuthenticationService>();
            authenticationService.Authenticate(_existingUser.Username, _existingUser.PasswordHash).Returns(_existingUser);
            authenticationService.Authenticate(_existingUser.Username, _notExistingUser.PasswordHash).Returns(ErrorType.AuthenticationFailed);
            authenticationService.Authenticate(_notExistingUser.Username, Arg.Any<string>()).Returns(ErrorType.NotFound);
            authenticationService.GenerateBearerToken(Arg.Any<string>()).Returns(BearerToken);

            var usersService = Substitute.For<IUsersService>();
            var deckManagementService = Substitute.For<IDeckManagementService>();

            var clientFactory = new ApiTestsWebApplicationFactory<Program>(authenticationService, usersService, deckManagementService);
            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task NoAuthorizationHeader_ShouldReturnUnauthorized()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "api/users/signin")
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task NotExistingUserCredentials_ShouldReturnUnauthorized()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "api/users/signin"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_notExistingUser.Username}:{_notExistingUser.PasswordHash}"))}" }
                }
            });

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ExistingUserId_ShouldReturnOkWithBearerToken()
        {
            var message = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress!, "api/users/signin"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_existingUser.Username}:{_existingUser.PasswordHash}"))}" }
                }
            });
            var response = await message.Content.ReadFromJsonAsync<SignInResponse>();

            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response!.Token.Should().Be(BearerToken);
        }
    }
}
