namespace PeculiarCardGame.WebApi.Models.Responses
{
    public class SignInResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
