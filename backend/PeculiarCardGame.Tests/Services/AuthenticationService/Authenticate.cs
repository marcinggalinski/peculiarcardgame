using System.Web.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using PeculiarCardGame.Shared.Options;
using Service = PeculiarCardGame.Services.Authentication.AuthenticationService;

namespace PeculiarCardGame.Tests.Services.AuthenticationService
{
    public class Authenticate
    {
        private const string Issuer = "test";
        private const string Key = "testtesttesttesttesttesttesttest";

        private const string ExistingUsername = "test";
        private const string NotExistingUsername = "invalid";
        private const string ValidPassword = "test";
        private const string InvalidPassword = "invalid";
        private const string InvalidToken = "invalid";
        
        private readonly IReadOnlyList<string> _audiences = new List<string> { "test" };

        private readonly User _user;

        private readonly IOptions<BearerTokenAuthenticationSchemeOptions> _options;
        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _notExistingUserFilledRequestContext;
        private readonly RequestContext _existingUserFilledRequestContext;

        public Authenticate()
        {
            const int UserId = 1;
            const string DisplayedName = "test";

            _user = new User
            {
                Id = UserId,
                Username = ExistingUsername,
                DisplayedName = DisplayedName,
                PasswordHash = Crypto.HashPassword(ValidPassword)
            };

            _options = Options.Create(new BearerTokenAuthenticationSchemeOptions
            {
                Audiences = _audiences,
                ClaimsIssuer = Issuer,
                Key = Key
            });

            _dbContext = TestHelpers.GetDbContext();

            _emptyRequestContext = new RequestContext();
            _notExistingUserFilledRequestContext = new RequestContext();
            _notExistingUserFilledRequestContext.SetOnce(new User
            {
                Username = NotExistingUsername,
                DisplayedName = "",
                PasswordHash = ""
            });
            _existingUserFilledRequestContext = new RequestContext();
            _existingUserFilledRequestContext.SetOnce(_user);
        }

        [Theory]
        [InlineData(null, ValidPassword)]
        [InlineData(ExistingUsername, null)]
        public void NullUsernameOrPassword_ShouldThrowArgumentNullException(string? username, string? password)
        {
            var service = new Service(_options, _dbContext, _emptyRequestContext);

#pragma warning disable CS8604
            var action = () => service.Authenticate(username, password);
#pragma warning restore CS8604

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(InvalidPassword)]
        [InlineData(ValidPassword)]
        public void NotExistingUsername_ShouldReturnErrorTypeNotFound(string password)
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_options, _dbContext, _emptyRequestContext);

            var result = service.Authenticate(NotExistingUsername, password);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.NotFound);
        }
        
        [Fact]
        public void InvalidPassword_ShouldReturnErrorTypeAuthenticationFailed()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_options, _dbContext, _emptyRequestContext);

            var result = service.Authenticate(ExistingUsername, InvalidPassword);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.AuthenticationFailed);
        }

        [Fact]
        public void ValidUsernameAndPassword_ShouldReturnUser()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_options, _dbContext, _emptyRequestContext);

            var result = service.Authenticate(_user.Username, ValidPassword);

            result.Should().BeRight();
            result.Right.Id.Should().Be(_user.Id);
            result.Right.Username.Should().Be(_user.Username);
            result.Right.DisplayedName.Should().Be(_user.DisplayedName);
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
        public void InvalidToken_ShouldReturnErrorTypeAuthenticationFailed()
        {
            var service = new Service(_options, _dbContext, _emptyRequestContext);

            var result = service.Authenticate(InvalidToken);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.AuthenticationFailed);
        }

        [Fact]
        public void ValidTokenForNotExistingUser_ShouldReturnErrorTypeNotfound()
        {
            var service = new Service(_options, _dbContext, _notExistingUserFilledRequestContext);
            var (token, _) = service.GenerateTokens(_audiences[0]);

            var result = service.Authenticate(token);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void ValidTokenForExistingUser_ShouldReturnUser()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_options, _dbContext, _existingUserFilledRequestContext);
            var (token, _) = service.GenerateTokens(_audiences[0]);

            var result = service.Authenticate(token);

            result.Should().BeRight();
            result.Right.Id.Should().Be(_user.Id);
            result.Right.Username.Should().Be(_user.Username);
            result.Right.DisplayedName.Should().Be(_user.DisplayedName);
        }
    }
}
