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

        /// <remarks>Requires <see cref="RequestContext"/> to be set.</remarks>
        /// <returns>A pair of access token and refresh token.</returns>
        (string AccessToken, string RefreshToken) GenerateTokens(string audience);

        /// <remarks>Requires <see cref="RequestContext"/> to not be set. Old refresh token is revoked.</remarks>
        /// <returns>
        /// A new pair of access token and refresh token.
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.AuthenticationFailed"/>
        /// </returns>
        Either<ErrorType, (string AccessToken, string RefreshToken)> RefreshTokens(string refreshToken, string audience);

        void RevokeRefreshToken(string refreshToken);
    }
}
