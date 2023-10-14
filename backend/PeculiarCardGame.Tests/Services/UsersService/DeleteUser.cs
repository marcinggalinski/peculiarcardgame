using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.Users.UsersService;

namespace PeculiarCardGame.UnitTests.Services.UsersService
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

            var options = new DbContextOptionsBuilder<PeculiarCardGameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new PeculiarCardGameDbContext(options);

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
            catch { }
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
        public void NotExistingUser_ShouldReturnFalse()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var deleted = service.DeleteUser(_user.Id);

            deleted.Should().BeFalse();
        }

        [Fact]
        public void AnotherUser_ShouldNotDeleteUser()
        {
            _dbContext.SetupTest(_user, _anotherUser);
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.DeleteUser(_anotherUser.Id);
            var deletedUser = _dbContext.Users.Single(x =>  x.Id == _user.Id);
            var callingUser = _dbContext.Users.Single(x => x.Id == _anotherUser.Id);

            deletedUser.Should().NotBeNull();
            callingUser.Should().NotBeNull();
            _dbContext.Users.Should().HaveCount(userCountBefore);
        }

        [Fact]
        public void AnotherUser_ShouldReturnFalse()
        {
            _dbContext.SetupTest(_user, _anotherUser);
            var service = new Service(_dbContext, _filledRequestContext);

            var deleted = service.DeleteUser(_anotherUser.Id);

            deleted.Should().BeFalse();
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
        public void ExistingUser_ShouldReturnTrue()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _filledRequestContext);

            var deleted = service.DeleteUser(_user.Id);

            deleted.Should().BeTrue();
        }
    }
}
