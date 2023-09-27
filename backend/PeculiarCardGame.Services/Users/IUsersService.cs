using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.Services.Users
{
    public interface IUsersService
    {
        User? AddUser(string id, string username, string password);
        User? GetUser(string id);
        User? UpdateUser(string id, string? usernameUpdate, string? passwordUpdate);
        bool DeleteUser(string id);

        bool Authenticate(string id, string password);
    }
}
