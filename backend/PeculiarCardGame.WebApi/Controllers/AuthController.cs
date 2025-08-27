using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Shared;
using PeculiarCardGame.WebApi.Infrastructure.Authentication;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PeculiarCardGame.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Exposes auth-related endpoints.")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpPost("signin")]
    [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
    [SwaggerOperation("Signs user in.", "Requires valid basic authentication data to be sent in 'Authorization' header. Returns bearer token used for authentication in other endpoints and refresh token used for generating new access token.")]
    [SwaggerResponse(200, "Signed in", typeof(SignInResponse))]
    [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
    public ActionResult<SignInResponse> SignIn()
    {
        return Ok(new SignInResponse
        {
            AccessToken = Request.HttpContext.User.Claims.Single(x => x.Type == "AccessToken").Value,
            RefreshToken = Request.HttpContext.User.Claims.Single(x => x.Type == "RefreshToken").Value
        });
    }

    [HttpPost("refresh")]
    [SwaggerOperation("Generates new access and refresh token.")]
    [SwaggerResponse(200, "Tokens refreshed", typeof(SignInResponse))]
    [SwaggerResponse(401, "Invalid refresh token", typeof(string))]
    public ActionResult<SignInResponse> RefreshTokens(RefreshTokensRequest request)
    {
        var result = _authenticationService.RefreshTokens(request.RefreshToken, Request.Headers.Origin.ToString());
        if (result.IsRight)
        {
            var (accessToken, refreshToken) = result.Right;
            return Ok(new SignInResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        return result.Left! switch
        {
            ErrorType.AuthenticationFailed => Unauthorized(),
            _ => throw new UnreachableException($"result.Left = {result.Left.ToString()}")
        };
    }

    [HttpPost("revoke")]
    [SwaggerOperation("Revokes refresh token.", "Marks refresh token as used, causing it to not be valid for refreshing.1")]
    [SwaggerResponse(200, "Token revoked")]
    public IActionResult RevokeRefreshToken(RevokeRefreshTokenRequest request)
    {
        _authenticationService.RevokeRefreshToken(request.RefreshToken);
        return Ok();
    }
}