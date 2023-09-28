using Microsoft.AspNetCore.Authentication;

namespace PeculiarCardGame.Options
{
    public class BearerTokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string ConfigurationKey = "Authentication:Bearer";

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
    }
}
