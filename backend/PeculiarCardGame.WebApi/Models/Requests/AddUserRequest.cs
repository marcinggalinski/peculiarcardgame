namespace PeculiarCardGame.WebApi.Models.Requests
{
    public class AddUserRequest
    {
        public string Username { get; set; }
        public string? DisplayedName { get; set; }
        public string Password { get; set; }
    }
}
