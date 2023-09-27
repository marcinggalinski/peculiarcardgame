using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly PeculiarCardGameDbContext _dbContext;

        public UsersService(PeculiarCardGameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User? AddUser(string id, string username, string password)
        {
            throw new NotImplementedException();
        }

        public User? GetUser(string id)
        {
            throw new NotImplementedException();
        }

        public User? UpdateUser(string id, string? usernameUpdate, string? passwordUpdate)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(string id)
        {
            throw new NotImplementedException();
        }

        public bool Authenticate(string id, string password)
        {
            //throw new NotImplementedException();
            return true;
        }
    }
}
