using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Shared;

namespace PeculiarCardGame.Services.Authentication
{
    public interface IAuthenticationService
    {
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.AuthenticationFailed"/>,
        /// <see cref="ErrorType.NotFound"/>
        /// </returns>
        Either<ErrorType, User> Authenticate(string username, string password);
        
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.AuthenticationFailed"/>,
        /// <see cref="ErrorType.NotFound"/>
        /// </returns>
        Either<ErrorType, User> Authenticate(string token);

        /// <remarks>Requires request context to be set.</remarks>
        string GenerateBearerToken(string audience);
    }
}
