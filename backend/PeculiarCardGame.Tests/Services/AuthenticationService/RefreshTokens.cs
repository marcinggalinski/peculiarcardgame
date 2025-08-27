using FluentAssertions;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using PeculiarCardGame.Shared.Options;
using MSOptions = Microsoft.Extensions.Options.Options;
using Service = PeculiarCardGame.Services.Authentication.AuthenticationService;

namespace PeculiarCardGame.Tests.Services.AuthenticationService;

public class RefreshTokens
{
    private readonly IReadOnlyList<string> Audiences = new List<string> { Audience };
    private const string Audience = "test";
    private const string Issuer = "test";
    private const string Key = "testtesttesttest";
    private const string ValidToken = "valid";
    private const string ExpiredToken = "ExpiredToken";
    private const string RevokedToken = "revoked";
    private const string NotExistingToken = "invalid";
    
    private readonly IOptions<BearerTokenAuthenticationSchemeOptions> _options;
    private readonly PeculiarCardGameDbContext _dbContext;
    private readonly RequestContext _requestContext;
    private readonly TokenInfo _validTokenInfo;
    private readonly TokenInfo _expiredTokenInfo;
    private readonly TokenInfo _revokedTokenInfo;
    private readonly User _user;
    
    public RefreshTokens()
    {
        const int UserId = 1;
        const string Username = "test";
        const string DisplayedName = "test";
        const string PasswordHash = "test";

        _user = new User
        {
            Id = UserId,
            Username = Username,
            DisplayedName = DisplayedName,
            PasswordHash = PasswordHash
        };
        
        _options = MSOptions.Create(new BearerTokenAuthenticationSchemeOptions
        {
            Audiences = Audiences,
            ClaimsIssuer = Issuer,
            Key = Key
        });

        _dbContext = TestHelpers.GetDbContext();

        _requestContext = new RequestContext();

        _validTokenInfo = new TokenInfo
        {
            Token = ValidToken,
            ExpirationDateUtc = DateTime.UtcNow.AddHours(1),
            UserId = UserId
        };
        _expiredTokenInfo = new TokenInfo
        {
            Token = ExpiredToken,
            ExpirationDateUtc = DateTime.UtcNow.AddHours(-1),
            UserId = UserId
        };
        _revokedTokenInfo = new TokenInfo
        {
            Token = RevokedToken,
            ExpirationDateUtc = DateTime.UtcNow.AddHours(1),
            IsRevoked = true,
            UserId = UserId
        };
    }

    [Theory]
    [InlineData(ExpiredToken)]
    [InlineData(RevokedToken)]
    [InlineData(NotExistingToken)]
    public void RefreshTokens_InvalidToken_ShouldReturnAuthenticationFailed(string token)
    {
        _dbContext.SetupTest(_user);
        _dbContext.SetupTest(_validTokenInfo, _expiredTokenInfo, _revokedTokenInfo);
        var service = new Service(_options, _dbContext, _requestContext);

        var result = service.RefreshTokens(token, Audience);

        result.Should().BeLeft();
        result.Left.Should().Be(ErrorType.AuthenticationFailed);
    }

    [Fact]
    public void RefreshToken_ValidToken_ShouldReturnNewTokenPair()
    {
        _dbContext.SetupTest(_user);
        _dbContext.SetupTest(_validTokenInfo, _expiredTokenInfo, _revokedTokenInfo);
        var service = new Service(_options, _dbContext, _requestContext);

        var result = service.RefreshTokens(ValidToken, Audience);
        
        result.Should().BeRight();
        var (accessToken, refreshToken) = result.Right;
        accessToken.Should().NotBeNull();
        refreshToken.Should().NotBeNull();
        refreshToken.Should().NotBe(ValidToken);
    }

    [Fact]
    public void RefreshToken_ValidToken_ShouldRevokeOldToken()
    {
        _dbContext.SetupTest(_user);
        _dbContext.SetupTest(_validTokenInfo, _expiredTokenInfo, _revokedTokenInfo);
        var service = new Service(_options, _dbContext, _requestContext);

        service.RefreshTokens(ValidToken, Audience);
        
        var tokenInfo = _dbContext.TokenInfos.Single(x => x.Token == ValidToken);
        tokenInfo.IsRevoked.Should().BeTrue();
    }
}