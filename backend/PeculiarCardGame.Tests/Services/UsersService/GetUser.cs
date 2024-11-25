using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
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
        public void NotExistingUserId_ShouldReturnErrorTypeNotFound()
        {
            var service = new Service(_dbContext, _requestContext);

            var result = service.GetUser(_user.Id);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void ExistingUserId_ShouldReturnUser()
        {
            _dbContext.SetupTest(_user);
            var service = new Service(_dbContext, _requestContext);

            var result = service.GetUser(_user.Id);

            result.Should().BeRight();
            result.Right.Username.Should().Be(_user.Username);
            result.Right.DisplayedName.Should().Be(_user.DisplayedName);
            result.Right.PasswordHash.Should().Be(_user.PasswordHash);
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
