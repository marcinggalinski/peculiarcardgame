using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.WebApi.Models.Responses
{
    public class GetUserResponse
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string DisplayedName { get; set; }

        public static GetUserResponse FromUser(User user)
        {
            return new GetUserResponse
            {
                Id = user.Id,
                Username = user.Username,
                DisplayedName = user.DisplayedName
            };
        }
    }
}
