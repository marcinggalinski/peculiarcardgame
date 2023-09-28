using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.Services.Authentication
{
    public interface IAuthenticationService
    {
        User? Authenticate(string username, string password);
        User? Authenticate(string token);

        /// <remarks>Requires request context to be set.</remarks>
        string GenerateBearerToken();
    }
}
