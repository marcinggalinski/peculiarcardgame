using Microsoft.AspNetCore.Authentication;

namespace PeculiarCardGame.Options
{
    public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string ConfigurationKey = "Authentication:Basic";
    }
}
