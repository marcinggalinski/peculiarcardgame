using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.Users.UsersService;

namespace PeculiarCardGame.Tests.Services.UsersService
{
    public class GetUser
    {
        private const int ExistingUserId = 1;
        private const int NotExistingUserId = 2;

        private readonly User _user;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public GetUser()
        {
            const string Username = "test";
            const string DisplayedName = "test";
            const string PasswordHash = "test";

            _user = new User
            {
                Id = ExistingUserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = PasswordHash
            };

            _dbContext = TestHelpers.GetDbContext();

            _requestContext = new RequestContext();
        }

        [Fact]
        public void NotExistingUserId_ShouldReturnNull()
        {
            var service = new Service(_dbContext, _requestContext);

            var user = service.GetUser(_user.Id);

            user.Should().BeNull();
        }

        [Fact]
        public void ExistingUserId_ShouldReturnUser()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _requestContext);

            var user = service.GetUser(_user.Id);

            user.Should().NotBeNull();
            user!.Username.Should().Be(_user.Username);
            user.DisplayedName.Should().Be(_user.DisplayedName);
            user.PasswordHash.Should().Be(_user.PasswordHash);
        }

        [Theory]
        [InlineData(ExistingUserId)]
        [InlineData(NotExistingUserId)]
        public void ShouldNotChangeUserCount(int userId)
        {
            _dbContext.SetupTest(_user);
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _requestContext);

            service.GetUser(userId);

            _dbContext.Users.Should().HaveCount(userCountBefore);
        }
    }
}
