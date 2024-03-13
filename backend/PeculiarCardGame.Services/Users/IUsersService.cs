using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.Services.Users
{
    public interface IUsersService
    {
        /// <remarks>Requires request context to not be set.</remarks>
        User? AddUser(string username, string? displayedName, string password);
        User? GetUser(int userId);
        /// <remarks>Requires request context to be set.</remarks>
        User? UpdateUser(int userId, string? displayedNameUpdate, string? passwordUpdate);
        /// <remarks>Requires request context to be set.</remarks>
        bool DeleteUser(int userId);
    }
}
