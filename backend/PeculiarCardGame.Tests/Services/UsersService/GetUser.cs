using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using Service = PeculiarCardGame.Services.Users.UsersService;

namespace PeculiarCardGame.UnitTests.Services.UsersService
{
    public class GetUser
    {
        private const string ExistingUsername = "test";
        private const string DisplayedName = "test";
        private const string NotExistingUsername = "invalid";

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public GetUser()
        {
            var options = new DbContextOptionsBuilder<PeculiarCardGameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new PeculiarCardGameDbContext(options);

            _requestContext = new RequestContext();
        }

        [Fact]
        public void NotExistingUsername_ShouldReturnNull()
        {
            var service = new Service(_dbContext, _requestContext);

            var user = service.GetUser(NotExistingUsername);

            user.Should().BeNull();
        }

        [Fact]
        public void ExistingUsername_ShouldReturnUser()
        {
            _dbContext.Users.Add(new User
            {
                Username = ExistingUsername,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _dbContext.SaveChanges();
            var service = new Service(_dbContext, _requestContext);

            var user = service.GetUser(ExistingUsername);

            user.Should().NotBeNull();
            user!.Username.Should().Be(ExistingUsername);
            user!.DisplayedName.Should().Be(DisplayedName);
        }

        [Fact]
        public void ExistingUsername_ShouldNotChangeUserCount()
        {
            _dbContext.Users.Add(new User
            {
                Username = ExistingUsername,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _dbContext.SaveChanges();
            var userCountBefore = _dbContext.Users.Count();
            var service = new Service(_dbContext, _requestContext);

            service.GetUser(ExistingUsername);

            _dbContext.Users.Should().HaveCount(userCountBefore);
        }
    }
}
