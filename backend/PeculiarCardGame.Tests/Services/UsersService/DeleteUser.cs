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
        private const string Username = "test";
        private const string DisplayedName = "test";
        private const string Password = "test";
        private const string AnotherUsername = "another";

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public DeleteUser()
        {
            var options = new DbContextOptionsBuilder<PeculiarCardGameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new PeculiarCardGameDbContext(options);

            _emptyRequestContext = new RequestContext();
            _filledRequestContext = new RequestContext();
            _filledRequestContext.SetOnce(new User
            {
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
        }

        [Fact]
        public void NullUsername_ShouldThrowArgumentNullException()
        {
            var service = new Service(_dbContext, _filledRequestContext);

#pragma warning disable CS8625
            var action = () => service.DeleteUser(null);
#pragma warning restore CS8625

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            var service = new Service(_dbContext, _emptyRequestContext);

            var action = () => service.DeleteUser(Username);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void FilledRequestContextAndNotExistingUser_ShouldNotDelete()
        {
            _dbContext.Users.Add(new User
            {
                Username = AnotherUsername,
                DisplayedName = AnotherUsername,
                PasswordHash = ""
            });
            _dbContext.SaveChanges();
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.DeleteUser(Username);

            _dbContext.Users.Should().HaveCount(userCountBefore);
        }

        [Fact]
        public void FilledRequestContextAndNotExistingUser_ShouldReturnFalse()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var deleted = service.DeleteUser(Username);

            deleted.Should().BeFalse();
        }

        [Fact]
        public void FilledRequestContextAndAnotherUser_ShouldNotDeleteUser()
        {
            _dbContext.Users.Add(new User
            {
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _dbContext.Users.Add(new User
            {
                Username = AnotherUsername,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _dbContext.SaveChanges();
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.DeleteUser(AnotherUsername);

            _dbContext.Users.Should().HaveCount(userCountBefore);
        }

        [Fact]
        public void FilledRequestContextAndAnotherUser_ShouldReturnFalse()
        {
            _dbContext.Users.Add(new User
            {
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _dbContext.Users.Add(new User
            {
                Username = AnotherUsername,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _dbContext.SaveChanges();
            var service = new Service(_dbContext, _filledRequestContext);

            var deleted = service.DeleteUser(AnotherUsername);

            deleted.Should().BeFalse();
        }

        [Fact]
        public void FilledRequestContextAndExistingUser_ShouldDeleteUser()
        {
            _dbContext.Users.Add(new User
            {
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _dbContext.SaveChanges();
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.DeleteUser(Username);
            var user = _dbContext.Users.SingleOrDefault(x => x.Username == Username);

            _dbContext.Users.Should().HaveCount(userCountBefore - 1);
            user.Should().BeNull();
        }

        [Fact]
        public void FilledRequestContextAndExistingUser_ShouldReturnTrue()
        {
            _dbContext.Users.Add(new User
            {
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _dbContext.SaveChanges();
            var service = new Service(_dbContext, _filledRequestContext);

            var deleted = service.DeleteUser(Username);

            deleted.Should().BeTrue();
        }
    }
}
