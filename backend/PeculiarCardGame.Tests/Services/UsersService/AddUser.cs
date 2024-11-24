using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.Users.UsersService;

namespace PeculiarCardGame.Tests.Services.UsersService
{
    public class AddUser
    {
        private const string Username = "test";
        private const string Password = "test";

        private readonly User _user;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public AddUser()
        {
            const int UserId = 1;
            const string DisplayedName = "test";
            const string PasswordHash = "test";

            _user = new User
            {
                Id = UserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };

            _dbContext = TestHelpers.GetDbContext();

            _emptyRequestContext = new RequestContext();
            _filledRequestContext = new RequestContext();
            _filledRequestContext.SetOnce(_user);
        }

        [Theory]
        [InlineData(null, Password)]
        [InlineData(Username, null)]
        public void NullUsernameOrPassword_ShouldThrowNullArgumentException(string? username, string? password)
        {
            var service = new Service(_dbContext, _emptyRequestContext);

#pragma warning disable CS8604
            var action = () => service.AddUser(username, _user.DisplayedName, password);
#pragma warning restore CS8604

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(null, Password)]
        [InlineData(Username, null)]
        public void NullUsernameOrPassword_ShouldNotAddUser(string? username, string? password)
        {
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
#pragma warning disable CS8604
                service.AddUser(username, _user.DisplayedName, password);
#pragma warning restore CS8604
            }
            catch (ArgumentNullException) { }

            _dbContext.Users.Should().HaveCount(userCountBefore);
        }

        [Fact]
        public void FilledRequestContext_ShouldThrowInvalidOperationException()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var action = () => service.AddUser(_user.Username, _user.DisplayedName, Password);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void FilledRequestContext_ShouldNotAddUser()
        {
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            try
            {
                service.AddUser(_user.Username, _user.DisplayedName, Password);
            }
            catch (InvalidOperationException) { }

            _dbContext.Users.Should().HaveCount(userCountBefore);
        }

        [Fact]
        public void ExistingUsername_ShouldReturnNull()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _emptyRequestContext);

            var user = service.AddUser(_user.Username, _user.DisplayedName, Password);

            user.Should().BeNull();
        }

        [Fact]
        public void ExistingUsername_ShouldNotAddUser()
        {
            _dbContext.SetupTest(_user);
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            service.AddUser(_user.Username, _user.DisplayedName, Password);

            _dbContext.Users.Should().HaveCount(userCountBefore);
        }

        [Fact]
        public void NotExistingUsername_ShouldAddNewUser()
        {
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            service.AddUser(_user.Username, _user.DisplayedName, Password);
            var user = _dbContext.Users.Single(x => x.Username == _user.Username);

            _dbContext.Users.Should().HaveCount(userCountBefore + 1);
            user.Username.Should().Be(_user.Username);
            user.DisplayedName.Should().Be(_user.DisplayedName);
        }

        [Fact]
        public void NotExistingUsername_ShouldReturnNewUser()
        {
            var service = new Service(_dbContext, _emptyRequestContext);

            var user = service.AddUser(_user.Username, _user.DisplayedName, Password);

            user.Should().NotBeNull();
            user!.Username.Should().Be(_user.Username);
            user.DisplayedName.Should().Be(_user.DisplayedName);
        }
    }
}
