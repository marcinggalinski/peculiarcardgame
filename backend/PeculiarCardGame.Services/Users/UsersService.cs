using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Shared;
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

        public Either<ErrorType, User> AddUser(string username, string? displayedName, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (_requestContext.CallingUser is not null)
                throw new InvalidOperationException($"{nameof(AddUser)} cannot be called by an authenticated user.");

            username = username.Trim();
            displayedName = displayedName?.Trim();
            
            if (username.Length > User.MaxUsernameLength || displayedName?.Length > User.MaxDisplayNameLength)
                return ErrorType.ConstraintsNotMet;

            var user = _dbContext.Users.SingleOrDefault(x => x.Username == username);
            if (user is not null)
                return ErrorType.Conflict;

            user = _dbContext.Users.Add(new User
            {
                Username = username,
                DisplayedName = displayedName ?? username,
                PasswordHash = Crypto.HashPassword(password)
            }).Entity;
            _dbContext.SaveChanges();

            return user;
        }

        public Either<ErrorType, User> GetUser(int userId)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.Id == userId);
            if (user is null)
                return ErrorType.NotFound;
            return user;
        }

        public Either<ErrorType, User> UpdateUser(int userId, string? displayedNameUpdate, string? passwordUpdate)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateUser)} can only be called by an authenticated user.");

            if (userId != _requestContext.CallingUser.Id)
                return ErrorType.Unauthorized;

            var user = _dbContext.Users.SingleOrDefault(x => x.Id == userId);
            if (user is null)
                return ErrorType.NotFound;

            displayedNameUpdate = displayedNameUpdate?.Trim();
            if (displayedNameUpdate is not null && displayedNameUpdate.Length > User.MaxDisplayNameLength)
                return ErrorType.ConstraintsNotMet;

            if (!string.IsNullOrEmpty(displayedNameUpdate))
                user.DisplayedName = displayedNameUpdate;
            if (!string.IsNullOrEmpty(passwordUpdate))
                user.PasswordHash = Crypto.HashPassword(passwordUpdate);

            user = _dbContext.Users.Update(user).Entity;
            _dbContext.SaveChanges();

            return user;
        }

        // TODO: Handle case when deleted user has decks
        public ErrorType? DeleteUser(int userId)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateUser)} can only be called by an authenticated user.");

            if (userId != _requestContext.CallingUser.Id)
                return ErrorType.Unauthorized;

            var user = _dbContext.Users.SingleOrDefault(x => x.Id == userId);
            if (user is null)
                return ErrorType.NotFound;

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
            
            return null;
        }
    }
}
