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

        public User? AddUser(string username, string? displayedName, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (_requestContext.CallingUser is not null)
                throw new InvalidOperationException($"{nameof(AddUser)} cannot be called by an authenticated user.");

            username = username.Trim();
            displayedName = displayedName?.Trim();

            var user = _dbContext.Users.SingleOrDefault(x => x.Username == username);
            if (user is not null)
                return null;

            user = _dbContext.Users.Add(new User
            {
                Username = username,
                DisplayedName = displayedName ?? username,
                PasswordHash = Crypto.HashPassword(password)
            }).Entity;
            _dbContext.SaveChanges();

            return user;
        }

        public User? GetUser(int userId)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.Id == userId);
            return user;
        }

        public User? UpdateUser(int userId, string? displayedNameUpdate, string? passwordUpdate)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateUser)} can only be called by an authenticated user.");

            if (userId != _requestContext.CallingUser.Id)
                return null;

            var user = _dbContext.Users.SingleOrDefault(x => x.Id == userId);
            if (user is null)
                return null;

            displayedNameUpdate = displayedNameUpdate?.Trim();
            passwordUpdate = passwordUpdate?.Trim();

            if (!string.IsNullOrEmpty(displayedNameUpdate))
                user.DisplayedName = displayedNameUpdate;
            if (!string.IsNullOrEmpty(passwordUpdate))
                user.PasswordHash = Crypto.HashPassword(passwordUpdate);

            user = _dbContext.Users.Update(user).Entity;
            _dbContext.SaveChanges();

            return user;
        }

        // TODO: Handle case when deleted user has decks
        public bool DeleteUser(int userId)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateUser)} can only be called by an authenticated user.");

            if (userId != _requestContext.CallingUser.Id)
                return false;

            var user = _dbContext.Users.SingleOrDefault(x => x.Id == userId);
            if (user is null)
                return false;

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
