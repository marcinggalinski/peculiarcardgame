using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NSubstitute;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.WebApi.Models.Requests;

namespace PeculiarCardGame.Tests.Controllers.AuthController;

public class RevokeRefreshToken
{
    private const string RefreshToken = "refresh";

    private readonly IAuthenticationService _authenticationService;
    private readonly HttpClient _client;

    public RevokeRefreshToken()
    {
        _authenticationService = Substitute.For<IAuthenticationService>();

        var clientFactory = new ApiTestsWebApplicationFactory<Program>(_authenticationService, null, null);
        _client = clientFactory.CreateClient();
    }

    [Fact]
    public async Task ShouldReturnOk()
    {
        var message = await _client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress!, "api/auth/revoke"),
            Content = JsonContent.Create(new RevokeRefreshTokenRequest
            {
                RefreshToken = RefreshToken
            })
        });

        message.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldAttemptToRevokeToken()
    {
        await _client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress!, "api/auth/revoke"),
            Content = JsonContent.Create(new RevokeRefreshTokenRequest
            {
                RefreshToken = RefreshToken
            })
        });

        _authenticationService.Received().RevokeRefreshToken(RefreshToken);
    }
}