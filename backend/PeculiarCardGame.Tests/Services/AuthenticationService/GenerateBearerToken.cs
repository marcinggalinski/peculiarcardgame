using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Options;
using PeculiarCardGame.Services;
using MSOptions = Microsoft.Extensions.Options.Options;
using Service = PeculiarCardGame.Services.Authentication.AuthenticationService;

namespace PeculiarCardGame.UnitTests.Services.AuthenticationService
{
    public class GenerateBearerToken
    {
        private const string TestAudience = "test";
        private const string TestIssuer = "test";
        private const string TestKey = "testtesttesttest";

        private const string TestUserUsername = "test";
        private const string TestUserDisplayedName = "test";

        private readonly IOptions<BearerTokenAuthenticationSchemeOptions> _options;
        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public GenerateBearerToken()
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
            _filledRequestContext = new RequestContext();
            _filledRequestContext.SetOnce(new User
            {
                Username = TestUserUsername,
                DisplayedName = TestUserDisplayedName,
                PasswordHash = ""
            });
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            var service = new Service(_options, _dbContext, _emptyRequestContext);

            var action = () => service.GenerateBearerToken();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void FilledRequestContext_ShouldReturnToken()
        {
            var service = new Service(_options, _dbContext, _filledRequestContext);

            var token = service.GenerateBearerToken();

            token.Should().NotBeNull();
        }
    }
}
