using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using PeculiarCardGame.Services;
using PeculiarCardGame.Services.Users;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace PeculiarCardGame.WebApi.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
    {
        public const string SchemeName = "Basic";

        private readonly IUsersService _usersService;
        private readonly RequestContext _requestContext;

        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUsersService usersService,
            RequestContext requestContext) : base(options, logger, encoder, clock)
        {
            _usersService = usersService;
            _requestContext = requestContext;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!AuthenticationHeaderValue.TryParse(Request.Headers[HeaderNames.Authorization], out var authentication))
                return Task.FromResult(AuthenticateResult.Fail("Missing authorization headers"));

            if (authentication.Scheme != SchemeName)
                return Task.FromResult(AuthenticateResult.Fail("Invalid authentication scheme"));

            if (authentication.Parameter is null)
                return Task.FromResult(AuthenticateResult.Fail("Missing credentials"));

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authentication.Parameter)).Split(':');
            if (credentials.Length != 2)
                return Task.FromResult(AuthenticateResult.Fail("Invalid parameter"));

            if (!_usersService.Authenticate(credentials[0], credentials[1]))
                return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));

            var user = _usersService.GetUser(credentials[0])!;
            _requestContext.SetOnce(user);

            var identity = new ClaimsIdentity(SchemeName);
            identity.AddClaim(new Claim("UserId", credentials[0]));

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
