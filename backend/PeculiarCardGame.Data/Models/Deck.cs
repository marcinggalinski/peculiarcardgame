using System.ComponentModel.DataAnnotations;

namespace PeculiarCardGame.Data.Models
{
    public class Deck
    {
        public const int MaxNameLength = 50;
        public const int MaxDescriptionLength = 500;

        public int Id { get; set; }
        public int AuthorId { get; set; }
        [MaxLength(MaxNameLength)]
        public required string Name { get; set; }
        [MaxLength(MaxDescriptionLength)]
        public required string Description { get; set; }

        public virtual User Author { get; set; } = null!;
        public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}
