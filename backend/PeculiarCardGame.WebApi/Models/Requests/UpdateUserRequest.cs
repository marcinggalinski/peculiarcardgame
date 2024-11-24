using System.ComponentModel.DataAnnotations;
using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.WebApi.Models.Requests
{
    public class UpdateUserRequest
    {
        [MaxLength(User.MaxDisplayNameLength)]
        public string? DisplayedNameUpdate { get; set; }
        public string? PasswordUpdate { get; set; }
    }
}
