using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Shared;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;

namespace PeculiarCardGame.Tests.Controllers.AuthController;

public class RefreshTokens
{
    private const string ValidRefreshToken = "valid refresh";
    private const string InvalidRefreshToken = "invalid refresh";
    private const string AccessToken = "access";
    private const string RefreshToken = "refresh";

    private readonly IAuthenticationService _authenticationService;
    private readonly HttpClient _client;

    public RefreshTokens()
    {
        _authenticationService = Substitute.For<IAuthenticationService>();
        _authenticationService.RefreshTokens(Arg.Any<string>(), Arg.Any<string>())
            .Returns(ErrorType.AuthenticationFailed);
        _authenticationService.RefreshTokens(ValidRefreshToken, Arg.Any<string>()).Returns((AccessToken, RefreshToken));

        var clientFactory = new ApiTestsWebApplicationFactory<Program>(_authenticationService, null, null);
        _client = clientFactory.CreateClient();
    }

    [Fact]
    public async Task InvalidRefreshToken_ShouldReturnUnauthorized()
    {
        var message = await _client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress!, "api/auth/refresh"),
            Content = JsonContent.Create(new RefreshTokensRequest
            {
                RefreshToken = InvalidRefreshToken
            })
        });

        message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRefreshToken_ShouldReturnOkWithTokenPair()
    {
        var message = await _client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress!, "api/auth/refresh"),
            Content = JsonContent.Create(new RefreshTokensRequest
            {
                RefreshToken = ValidRefreshToken
            })
        });
        var response = await message.Content.ReadFromJsonAsync<SignInResponse>();

        message.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response!.AccessToken.Should().Be(AccessToken);
        response.RefreshToken.Should().Be(RefreshToken);
    }

    [Fact]
    public async Task ShouldAttemptToRefreshTokens()
    {
        await _client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress!, "api/auth/refresh"),
            Content = JsonContent.Create(new RefreshTokensRequest
            {
                RefreshToken = ValidRefreshToken
            })
        });

        _authenticationService.Received().RefreshTokens(ValidRefreshToken, Arg.Any<string>());
    }
}