using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using Service = PeculiarCardGame.Services.Users.UsersService;

namespace PeculiarCardGame.Tests.Services.UsersService
{
    public class DeleteUser
    {
        private readonly User _user;
        private readonly User _anotherUser;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public DeleteUser()
        {
            const int UserId = 1;
            const int AnotherUserId = 2;
            const string Username = "test";
            const string AnotherUsername = "another";
            const string DisplayedName = "test";
            const string PasswordHash = "test";

            _user = new User
            {
                Id = UserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };
            _anotherUser = new User
            {
                Id = AnotherUserId,
                Username = AnotherUsername,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };

            _dbContext = TestHelpers.GetDbContext();

            _emptyRequestContext = new RequestContext();
            _filledRequestContext = new RequestContext();
            _filledRequestContext.SetOnce(_user);
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _emptyRequestContext);

            var action = () => service.DeleteUser(_user.Id);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldNotDeleteUser()
        {
            _dbContext.SetupTest(_user);
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
                service.DeleteUser(_user.Id);
            }
            catch (InvalidOperationException) { }
            var user = _dbContext.Users.Single(x => x.Id == _user.Id);

            user.Should().NotBeNull();
            _dbContext.Users.Should().HaveCount(userCountBefore);
        }

        [Fact]
        public void NotExistingUser_ShouldNotDeleteUser()
        {
            _dbContext.SetupTest(_anotherUser);
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.DeleteUser(_user.Id);
            var user = _dbContext.Users.Single(x => x.Id == _anotherUser.Id);

            user.Should().NotBeNull();
            _dbContext.Users.Should().HaveCount(userCountBefore);
        }

        [Fact]
        public void NotExistingUser_ShouldReturnErrorTypeNotFound()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var error = service.DeleteUser(_user.Id);

            error.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void AnotherUser_ShouldNotDeleteUser()
        {
            _dbContext.SetupTest(_user, _anotherUser);
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.DeleteUser(_anotherUser.Id);
            var deletedUser = _dbContext.Users.Single(x => x.Id == _user.Id);
            var callingUser = _dbContext.Users.Single(x => x.Id == _anotherUser.Id);

            deletedUser.Should().NotBeNull();
            callingUser.Should().NotBeNull();
            _dbContext.Users.Should().HaveCount(userCountBefore);
        }

        [Fact]
        public void AnotherUser_ShouldReturnErrorTypeUnauthorized()
        {
            _dbContext.SetupTest(_user, _anotherUser);
            var service = new Service(_dbContext, _filledRequestContext);

            var error = service.DeleteUser(_anotherUser.Id);

            error.Should().NotBeNull();
            error.Should().Be(ErrorType.Unauthorized);
        }

        [Fact]
        public void ExistingUser_ShouldDeleteUser()
        {
            _dbContext.SetupTest(_user);
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.DeleteUser(_user.Id);
            var user = _dbContext.Users.SingleOrDefault(x => x.Id == _user.Id);

            _dbContext.Users.Should().HaveCount(userCountBefore - 1);
            user.Should().BeNull();
        }

        [Fact]
        public void ExistingUser_ShouldReturnNull()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _filledRequestContext);

            var error = service.DeleteUser(_user.Id);

            error.Should().BeNull();
        }
    }
}
