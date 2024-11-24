using System.ComponentModel.DataAnnotations;
using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.WebApi.Models.Requests
{
    public class AddUserRequest
    {
        [MaxLength(User.MaxUsernameLength)]
        public required string Username { get; set; }
        [MaxLength(User.MaxDisplayNameLength)]
        public string? DisplayedName { get; set; }
        public required string Password { get; set; }
    }
}
