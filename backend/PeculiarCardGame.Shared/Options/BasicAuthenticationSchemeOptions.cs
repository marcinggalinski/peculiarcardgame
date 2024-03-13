using Microsoft.AspNetCore.Authentication;

namespace PeculiarCardGame.Shared.Options
{
    public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string ConfigurationKey = "Authentication:Basic";
    }
}
