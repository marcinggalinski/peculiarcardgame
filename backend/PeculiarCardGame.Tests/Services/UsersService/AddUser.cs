using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.Users.UsersService;

namespace PeculiarCardGame.UnitTests.Services.UsersService
{
    public class AddUser
    {
        private const string Username = "test";
        private const string DisplayedName = "test";
        private const string Password = "test";

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public AddUser()
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

        [Theory]
        [InlineData(null, DisplayedName, Password)]
        [InlineData(Username, null, Password)]
        [InlineData(Username, DisplayedName, null)]
        public void NullUsernameOrDisplayedNameOrPassword_ShouldThrowNullArgumentException(string? username, string? displayedName, string? password)
        {
            var service = new Service(_dbContext, _emptyRequestContext);

#pragma warning disable CS8604
            var action = () => service.AddUser(username, displayedName, password);
#pragma warning restore CS8604

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FilledRequestContext_ShouldThrowInvalidOperationException()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var action = () => service.AddUser(Username, DisplayedName, Password);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContextAndExistingUsername_ShouldReturnNull()
        {
            _dbContext.Users.Add(new User
            {
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _dbContext.SaveChanges();
            var service = new Service(_dbContext, _emptyRequestContext);

            var user = service.AddUser(Username, DisplayedName, Password);

            user.Should().BeNull();
            _dbContext.Users.Count(x => x.Username == Username).Should().Be(1);
        }

        [Fact]
        public void EmptyRequestContextAndNotExistingUsername_ShouldAddNewUser()
        {
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            service.AddUser(Username, DisplayedName, Password);
            var user = _dbContext.Users.SingleOrDefault(x => x.Username == Username);

            _dbContext.Users.Should().HaveCount(userCountBefore + 1);
            user.Should().NotBeNull();
            user!.Username.Should().Be(Username);
            user!.DisplayedName.Should().Be(DisplayedName);
        }

        [Fact]
        public void EmptyRequestContextAndNotExistingUsername_ShouldReturnNewUser()
        {
            var service = new Service(_dbContext, _emptyRequestContext);

            var user = service.AddUser(Username, DisplayedName, Password);

            user.Should().NotBeNull();
            user!.Username.Should().Be(Username);
            user!.DisplayedName.Should().Be(DisplayedName);
        }
    }
}
