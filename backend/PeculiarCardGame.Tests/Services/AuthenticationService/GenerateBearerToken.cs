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
        private const string Audience = "test";
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
                Audience = Audience,
                Issuer = Issuer,
                Key = Key
            });

            var options = new DbContextOptionsBuilder<PeculiarCardGameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new PeculiarCardGameDbContext(options);

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
