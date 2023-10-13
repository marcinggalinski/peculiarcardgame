using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Options;
using PeculiarCardGame.Services;
using PeculiarCardGame.Services.Users;
using MSOptions = Microsoft.Extensions.Options.Options;
using Service = PeculiarCardGame.Services.Authentication.AuthenticationService;

namespace PeculiarCardGame.UnitTests.Services.AuthenticationService
{
    public class Authenticate
    {
        private const string TestAudience = "test";
        private const string TestIssuer = "test";
        private const string TestKey = "testtesttesttest";

        private const string TestUserUsername = "test";
        private const string TestUserDisplayedName = "test";
        private const string TestUserPassword = "test";

        private const string InvalidUsername = "invalid";
        private const string InvalidPassword = "invalid";
        private const string InvalidToken = "invalid";

        private readonly IOptions<BearerTokenAuthenticationSchemeOptions> _options;
        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _notExistingUserFilledRequestContext;
        private readonly RequestContext _existingUserFilledRequestContext;

        private readonly IUsersService _usersService;

        public Authenticate()
        {
            _options = MSOptions.Create(new BearerTokenAuthenticationSchemeOptions
            {
                Audience = TestAudience,
                Issuer = TestIssuer,
                Key = TestKey
            });

            var options = new DbContextOptionsBuilder<PeculiarCardGameDbContext>()
                .UseInMemoryDatabase("PeculiarCardGame")
                .Options;
            _dbContext = new PeculiarCardGameDbContext(options);

            _emptyRequestContext = new RequestContext();
            _notExistingUserFilledRequestContext = new RequestContext();
            _notExistingUserFilledRequestContext.SetOnce(new User
            {
                Username = InvalidUsername,
                DisplayedName = "",
                PasswordHash = ""
            });
            _existingUserFilledRequestContext = new RequestContext();
            _existingUserFilledRequestContext.SetOnce(new User
            {
                Username = TestUserUsername,
                DisplayedName = TestUserDisplayedName,
                PasswordHash = ""
            });

            _usersService = new UsersService(_dbContext, _emptyRequestContext);
        }

        [Theory]
        [InlineData(null, TestUserPassword)]
        [InlineData(TestUserUsername, null)]
        public void NullUsernameOrPassword_ShouldThrowArgumentNullException(string? username, string? password)
        {
            var service = new Service(_options, _dbContext, _emptyRequestContext);

#pragma warning disable CS8604
            var action = () => service.Authenticate(username, password);
#pragma warning restore CS8604

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(InvalidUsername, TestUserPassword)]
        [InlineData(TestUserUsername, InvalidPassword)]
        public void InvalidUsernameOrPassword_ShouldReturnNull(string username, string password)
        {
            var service = new Service(_options, _dbContext, _emptyRequestContext);

            var user = service.Authenticate(username, password);

            user.Should().BeNull();
        }

        [Fact]
        public void ValidUsernameAndPassword_ShouldReturnUser()
        {
            _usersService.AddUser(TestUserUsername, TestUserDisplayedName, TestUserPassword);
            var service = new Service(_options, _dbContext, _emptyRequestContext);

            var user = service.Authenticate(TestUserUsername, TestUserPassword);

            user!.Should().NotBeNull();
            user!.Username.Should().Be(TestUserUsername);
            user!.DisplayedName.Should().Be(TestUserDisplayedName);
        }

        [Fact]
        public void NullToken_ShouldThrowArgumentNullException()
        {
            var service = new Service(_options, _dbContext, _emptyRequestContext);

#pragma warning disable CS8625
            var action = () => service.Authenticate(null);
#pragma warning restore CS8625

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void InvalidToken_ShouldReturnNull()
        {
            var service = new Service(_options, _dbContext, _emptyRequestContext);

            var user = service.Authenticate(InvalidToken);

            user.Should().BeNull();
        }

        [Fact]
        public void ValidTokenForNotExistingUser_ShouldReturnNull()
        {
            var service = new Service(_options, _dbContext, _notExistingUserFilledRequestContext);
            var token = service.GenerateBearerToken();

            var user = service.Authenticate(token);

            user.Should().BeNull();
        }

        [Fact]
        public void ValidTokenForExistingUser_ShouldReturnUser()
        {
            _usersService.AddUser(TestUserUsername, TestUserDisplayedName, TestUserPassword);
            var service = new Service(_options, _dbContext, _existingUserFilledRequestContext);
            var token = service.GenerateBearerToken();

            var user = service.Authenticate(token);

            user.Should().NotBeNull();
            user!.Username.Should().Be(TestUserUsername);
            user!.DisplayedName.Should().Be(TestUserDisplayedName);
        }
    }
}
