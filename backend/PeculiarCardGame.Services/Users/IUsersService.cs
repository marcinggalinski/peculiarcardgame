using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Shared;

namespace PeculiarCardGame.Services.Users
{
    public interface IUsersService
    {
        /// <remarks>Requires <see cref="RequestContext"/> to not be set.</remarks>
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.Conflict"/>,
        /// <see cref="ErrorType.ConstraintsNotMet"/>
        /// </returns>
        Either<ErrorType, User> AddUser(string username, string? displayedName, string password);
        
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.NotFound"/>
        /// </returns>
        Either<ErrorType, User> GetUser(int userId);
        
        /// <remarks>Requires <see cref="RequestContext"/> to be set.</remarks>
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.ConstraintsNotMet"/>,
        /// <see cref="ErrorType.NotFound"/>,
        /// <see cref="ErrorType.Unauthorized"/>
        /// </returns>
        Either<ErrorType, User> UpdateUser(int userId, string? displayedNameUpdate, string? passwordUpdate);
        
        /// <remarks>Requires <see cref="RequestContext"/> to be set.</remarks>
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.NotFound"/>,
        /// <see cref="ErrorType.Unauthorized"/>
        /// </returns>
        ErrorType? DeleteUser(int userId);
    }
}
