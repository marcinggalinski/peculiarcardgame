using Microsoft.AspNetCore.Authentication;

namespace PeculiarCardGame.Shared.Options
{
    public class BearerTokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string ConfigurationKey = "Authentication:Bearer";

        public IReadOnlyList<string> Audiences { get; set; }
        public string Key { get; set; }
    }
}
