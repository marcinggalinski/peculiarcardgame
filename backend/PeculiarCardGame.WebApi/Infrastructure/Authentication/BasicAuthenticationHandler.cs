using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using IAuthenticationService = PeculiarCardGame.Services.Authentication.IAuthenticationService;

namespace PeculiarCardGame.WebApi.Infrastructure.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
    {
        public const string SchemeName = "Basic";

        private readonly IAuthenticationService _authenticationService;
        private readonly RequestContext _requestContext;

        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IAuthenticationService authenticationService,
            RequestContext requestContext) : base(options, logger, encoder)
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
                return Task.FromResult(AuthenticateResult.Fail("Missing credentials"));

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authentication.Parameter)).Split(':');
            if (credentials.Length != 2)
                return Task.FromResult(AuthenticateResult.Fail("Invalid parameter"));

            var authenticationResult = _authenticationService.Authenticate(credentials[0], credentials[1]);
            if (authenticationResult.IsLeft)
                return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));

            _requestContext.SetOnce(authenticationResult.Right);
            var (accessToken, refreshToken) = _authenticationService.GenerateTokens(Request.Headers.Origin.ToString());

            var identity = new ClaimsIdentity(SchemeName);
            identity.AddClaim(new Claim("AccessToken", accessToken));
            identity.AddClaim(new Claim("RefreshToken", refreshToken));
            identity.AddClaim(new Claim("UserId", credentials[0]));

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
