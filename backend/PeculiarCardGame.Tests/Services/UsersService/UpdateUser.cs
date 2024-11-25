using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using Service = PeculiarCardGame.Services.Users.UsersService;

namespace PeculiarCardGame.Tests.Services.UsersService
{
    public class UpdateUser
    {
        const string NewDisplayedName = "new";
        const string NewPassword = "new";

        private readonly User _user;
        private readonly User _anotherUser;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public UpdateUser()
        {
            const int UserId = 1;
            const int AnotherUserId = 2;
            const string Username = "test";
            const string DisplayedName = "test";
            const string PasswordHash = "test";
            const string AnotherUsername = "another";

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

            var action = () => service.UpdateUser(_user.Id, _user.DisplayedName, _user.PasswordHash);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldNotUpdateUser()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
                service.UpdateUser(_user.Id, _user.DisplayedName, _user.PasswordHash);
            }
            catch (InvalidOperationException) { }
            var user = _dbContext.Users.Single(x => x.Id == _user.Id);

            user.Id.Should().Be(_user.Id);
            user.Username.Should().Be(_user.Username);
            user.DisplayedName.Should().Be(_user.DisplayedName);
            user.PasswordHash.Should().Be(_user.PasswordHash);
        }

        [Fact]
        public void NotExistingUser_ShouldReturnErrorTypeNotFound()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var result = service.UpdateUser(_user.Id, null, null);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void NotExistingUser_ShouldNotUpdateUser()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _filledRequestContext);

            service.UpdateUser(_user.Id, null, null);
            var user = _dbContext.Users.Single(x => x.Id == _user.Id);

            user.Id.Should().Be(_user.Id);
            user.Username.Should().Be(_user.Username);
            user.DisplayedName.Should().Be(_user.DisplayedName);
            user.PasswordHash.Should().Be(_user.PasswordHash);
        }

        [Fact]
        public void AnotherUser_ShouldReturnErrorTypeUnauthorized()
        {
            _dbContext.SetupTest(_user, _anotherUser);
            var service = new Service(_dbContext, _filledRequestContext);

            var result = service.UpdateUser(_anotherUser.Id, null, null);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.Unauthorized);
        }

        [Fact]
        public void AnotherUser_ShouldNotUpdateUser()
        {
            _dbContext.SetupTest(_user, _anotherUser);
            var service = new Service(_dbContext, _filledRequestContext);

            service.UpdateUser(_anotherUser.Id, null, null);
            var user = _dbContext.Users.Single(x => x.Id == _user.Id);

            user.Id.Should().Be(_user.Id);
            user.Username.Should().Be(_user.Username);
            user.DisplayedName.Should().Be(_user.DisplayedName);
            user.PasswordHash.Should().Be(_user.PasswordHash);
        }

        [Theory]
        [InlineData(NewDisplayedName, null)]
        [InlineData(null, NewPassword)]
        public void ExistingUser_ShouldUpdateUser(string? displayedNameUpdate, string? passwordUpdate)
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _filledRequestContext);

            service.UpdateUser(_user.Id, displayedNameUpdate, passwordUpdate);
            var user = _dbContext.Users.Single(x => x.Username == _user.Username);

            user.Id.Should().Be(_user.Id);
            user.Username.Should().Be(_user.Username);
            user.DisplayedName.Should().Be(displayedNameUpdate ?? _user.DisplayedName);
            if (passwordUpdate is null)
                user.PasswordHash.Should().Be(_user.PasswordHash);
            else
                user.PasswordHash.Should().NotBe(_user.PasswordHash);
        }

        [Theory]
        [InlineData(NewDisplayedName, null)]
        [InlineData(null, NewPassword)]
        public void ExistingUser_ShouldReturnUpdatedUser(string? displayedNameUpdate, string? passwordUpdate)
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _filledRequestContext);

            var result = service.UpdateUser(_user.Id, displayedNameUpdate, passwordUpdate);

            result.Should().BeRight();
            result.Right.Id.Should().Be(_user.Id);
            result.Right.Username.Should().Be(_user.Username);
            result.Right.DisplayedName.Should().Be(displayedNameUpdate ?? _user.DisplayedName);
            if (passwordUpdate is null)
                result.Right.PasswordHash.Should().Be(_user.PasswordHash);
            else
                result.Right.PasswordHash.Should().NotBe(_user.PasswordHash);
        }

        [Fact]
        public void ExistingUser_ShouldNotChangeUserCount()
        {
            _dbContext.SetupTest(_user);
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.UpdateUser(_user.Id, _user.DisplayedName, _user.PasswordHash);

            _dbContext.Users.Should().HaveCount(userCountBefore);
        }
    }
}
