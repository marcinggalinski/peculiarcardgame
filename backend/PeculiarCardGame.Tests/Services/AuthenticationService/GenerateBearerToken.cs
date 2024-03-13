using FluentAssertions;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared.Options;
using MSOptions = Microsoft.Extensions.Options.Options;
using Service = PeculiarCardGame.Services.Authentication.AuthenticationService;

namespace PeculiarCardGame.UnitTests.Services.AuthenticationService
{
    public class GenerateBearerToken
    {
        private readonly IReadOnlyList<string> Audiences = new List<string>() { "test" };
        private const string Issuer = "test";
        private const string Key = "testtesttesttest";

        private readonly IOptions<BearerTokenAuthenticationSchemeOptions> _options;
        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public GenerateBearerToken()
        {
            const string Username = "test";
            const string DisplayedName = "test";

            _options = MSOptions.Create(new BearerTokenAuthenticationSchemeOptions
            {
                Audiences = Audiences,
                ClaimsIssuer = Issuer,
                Key = Key
            });

            _dbContext = TestHelpers.GetDbContext();

            _emptyRequestContext = new RequestContext();
            _filledRequestContext = new RequestContext();
            _filledRequestContext.SetOnce(new User
            {
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            var service = new Service(_options, _dbContext, _emptyRequestContext);

            var action = () => service.GenerateBearerToken("");

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void NullAudience_ShouldThrowArgumentNullExceptionException()
        {
            var service = new Service(_options, _dbContext, _emptyRequestContext);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var action = () => service.GenerateBearerToken(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FilledRequestContextAndNotNullAudience_ShouldReturnToken()
        {
            var service = new Service(_options, _dbContext, _filledRequestContext);

            var token = service.GenerateBearerToken("");

            token.Should().NotBeNull();
        }
    }
}
