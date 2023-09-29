using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using System.Web.Helpers;

namespace PeculiarCardGame.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public UsersService(PeculiarCardGameDbContext dbContext, RequestContext requestContext)
        {
            _dbContext = dbContext;
            _requestContext = requestContext;
        }

        public User? AddUser(string username, string displayedName, string password)
        {
            if (_requestContext.CallingUser is not null)
                throw new InvalidOperationException($"{nameof(AddUser)} cannot be called by an authenticated user.");

            var user = _dbContext.Users.SingleOrDefault(x => x.Username == username);
            if (user is not null)
                return null;

            user = _dbContext.Users.Add(new User
            {
                Username = username,
                DisplayedName = displayedName,
                PasswordHash = Crypto.HashPassword(password)
            }).Entity;
            _dbContext.SaveChanges();

            return user;
        }

        public User? GetUser(int id)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.Id == id);
            return user;
        }

        public User? GetUser(string username)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.Username == username);
            return user;
        }

        public User? UpdateUser(string username, string? displayedNameUpdate, string? passwordUpdate)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateUser)} can only be called by an authenticated user.");

            if (username != _requestContext.CallingUser.Username)
                return null;

            var user = _dbContext.Users.SingleOrDefault(x => x.Username == username);
            if (user is null)
                return null;

            if (displayedNameUpdate is not null)
                user.DisplayedName = displayedNameUpdate;
            if (passwordUpdate is not null)
                user.PasswordHash = Crypto.HashPassword(passwordUpdate);

            user = _dbContext.Users.Update(user).Entity;
            _dbContext.SaveChanges();

            return user;
        }

        public bool DeleteUser(string username)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateUser)} can only be called by an authenticated user.");

            if (username != _requestContext.CallingUser.Username)
                return false;

            var user = _dbContext.Users.SingleOrDefault(x => x.Username == username);
            if (user is null)
                return false;

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
