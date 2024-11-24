using Microsoft.AspNetCore.Authentication;

namespace PeculiarCardGame.Shared.Options
{
    public class BearerTokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string ConfigurationKey = "Authentication:Bearer";

        public IReadOnlyList<string> Audiences { get; set; } = new List<string>();
        public string Key { get; set; } = null!;
    }
}
