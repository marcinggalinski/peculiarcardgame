using FluentAssertions;
using Microsoft.Extensions.Options;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared.Options;
using MSOptions = Microsoft.Extensions.Options.Options;
using Service = PeculiarCardGame.Services.Authentication.AuthenticationService;

namespace PeculiarCardGame.Tests.Services.AuthenticationService;

public class RevokeRefreshToken
{
    private readonly IReadOnlyList<string> Audiences = new List<string> { Audience };
    private const string Audience = "test";
    private const string Issuer = "test";
    private const string Key = "testtesttesttesttesttesttesttest";
    private const string Token = "valid";
    
    private readonly IOptions<BearerTokenAuthenticationSchemeOptions> _options;
    private readonly PeculiarCardGameDbContext _dbContext;
    private readonly RequestContext _requestContext;
    private readonly TokenInfo _tokenInfo;
    private readonly User _user;
    
    public RevokeRefreshToken()
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

        _tokenInfo = new TokenInfo
        {
            Token = Token,
            ExpirationDateUtc = DateTime.UtcNow.AddHours(1),
            UserId = UserId
        };
    }

    [Fact]
    public void RefreshToken_ShouldRevokeToken()
    {
        _dbContext.SetupTest(_user);
        _dbContext.SetupTest(_tokenInfo);
        var service = new Service(_options, _dbContext, _requestContext);

        service.RefreshTokens(Token, Audience);
        
        var tokenInfo = _dbContext.TokenInfos.Single(x => x.Token == Token);
        tokenInfo.IsRevoked.Should().BeTrue();
    }
}