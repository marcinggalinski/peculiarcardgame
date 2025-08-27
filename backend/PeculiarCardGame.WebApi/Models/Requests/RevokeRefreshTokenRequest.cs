namespace PeculiarCardGame.WebApi.Models.Requests;

public class RevokeRefreshTokenRequest
{
    public required string RefreshToken { get; set; }
}