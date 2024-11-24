using System.ComponentModel.DataAnnotations;

namespace PeculiarCardGame.Data.Models
{
    public class User
    {
        public const int MaxUsernameLength = 30;
        public const int MaxDisplayNameLength = 30;
        public const int PasswordHashLength = 68;

        public int Id { get; set; }
        [MaxLength(MaxUsernameLength)]
        public required string Username { get; set; }
        [MaxLength(MaxDisplayNameLength)]
        public required string DisplayedName { get; set; }
        [MaxLength(PasswordHashLength)]
        public required string PasswordHash { get; set; }

        public virtual ICollection<Deck> Decks { get; set; } = new List<Deck>();
    }
}
