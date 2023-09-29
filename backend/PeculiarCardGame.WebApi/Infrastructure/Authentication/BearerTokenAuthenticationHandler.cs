using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using PeculiarCardGame.Options;
using PeculiarCardGame.Services;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using IAuthenticationService = PeculiarCardGame.Services.Authentication.IAuthenticationService;

namespace PeculiarCardGame.WebApi.Infrastructure.Authentication
{
    public class BearerTokenAuthenticationHandler : AuthenticationHandler<BearerTokenAuthenticationSchemeOptions>
    {
        public const string SchemeName = "Bearer";

        private readonly IAuthenticationService _authenticationService;
        private readonly RequestContext _requestContext;

        public BearerTokenAuthenticationHandler(
            IOptionsMonitor<BearerTokenAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthenticationService authenticationService,
            RequestContext requestContext) : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
            _requestContext = requestContext;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!AuthenticationHeaderValue.TryParse(Request.Headers[HeaderNames.Authorization], out var authentication))
                return Task.FromResult(AuthenticateResult.Fail("Missing authorization headers"));

            if (authentication.Scheme != SchemeName)
                return Task.FromResult(AuthenticateResult.Fail("Invalid authentication scheme"));

            if (authentication.Parameter is null)
                return Task.FromResult(AuthenticateResult.Fail("Missing token"));

            var token = authentication.Parameter;
            var user = _authenticationService.Authenticate(token);
            if (user is null)
                return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));

            _requestContext.SetOnce(user);

            var identity = new ClaimsIdentity(SchemeName);
            identity.AddClaim(new Claim("BearerToken", token));
            identity.AddClaim(new Claim("UserId", user.Username));

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
