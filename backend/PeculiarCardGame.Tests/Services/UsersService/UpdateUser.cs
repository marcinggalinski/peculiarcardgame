using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.Users.UsersService;

namespace PeculiarCardGame.UnitTests.Services.UsersService
{
    public class UpdateUser
    {
        private const string Username = "test";
        private const string DisplayedName = "test";
        private const string Password = "test";
        private const string AnotherUsername = "another";
        private const string NewDisplayedName = "new";
        private const string NewPassword = "new";

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public UpdateUser()
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
            var action = () => service.UpdateUser(null, DisplayedName, Password);
#pragma warning restore CS8625

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            var service = new Service(_dbContext, _emptyRequestContext);

            var action = () => service.UpdateUser(Username, DisplayedName, Password);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void FilledRequestContextAndNotExistingUser_ShouldReturnNull()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var user = service.UpdateUser(Username, DisplayedName, Password);

            user.Should().BeNull();
        }

        [Fact]
        public void FilledRequestContextAndAnotherUser_ShouldReturnNull()
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

            var user = service.UpdateUser(AnotherUsername, DisplayedName, Password);

            user.Should().BeNull();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(NewDisplayedName, null)]
        [InlineData(null, NewPassword)]
        [InlineData(NewDisplayedName, NewPassword)]
        public void FilledRequestContextAndExistingUser_ShouldUpdateUser(string? displayedNameUpdate, string? passwordUpdate)
        {
            _dbContext.Users.Add(new User
            {
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = Password
            });
            _dbContext.SaveChanges();
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.UpdateUser(Username, displayedNameUpdate, passwordUpdate);
            var user = _dbContext.Users.SingleOrDefault(x => x.Username == Username);

            _dbContext.Users.Should().HaveCount(userCountBefore);
            user.Should().NotBeNull();
            user!.Username.Should().Be(Username);
            user!.DisplayedName.Should().Be(displayedNameUpdate ?? DisplayedName);
            if (passwordUpdate is null)
                user!.PasswordHash.Should().Be(Password);
            else
                user!.PasswordHash.Should().NotBe(Password);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(NewDisplayedName, null)]
        [InlineData(null, NewPassword)]
        [InlineData(NewDisplayedName, NewPassword)]
        public void FilledRequestContextAndExistingUser_ShouldReturnUpdatedUser(string? displayedNameUpdate, string? passwordUpdate)
        {
            _dbContext.Users.Add(new User
            {
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = Password
            });
            _dbContext.SaveChanges();
            var service = new Service(_dbContext, _filledRequestContext);

            var user = service.UpdateUser(Username, displayedNameUpdate, passwordUpdate);

            user.Should().NotBeNull();
            user!.Username.Should().Be(Username);
            user!.DisplayedName.Should().Be(displayedNameUpdate ?? DisplayedName);
            if (passwordUpdate is null)
                user!.PasswordHash.Should().Be(Password);
            else
                user!.PasswordHash.Should().NotBe(Password);
        }
    }
}
