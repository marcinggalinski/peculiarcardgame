using System.ComponentModel.DataAnnotations;
using PeculiarCardGame.Shared;

namespace PeculiarCardGame.Data.Models
{
    public class Card
    {
        public const int MaxTextLength = 60;

        public int Id { get; set; }
        public required int DeckId { get; set; }
        [MaxLength(MaxTextLength)]
        public required string Text { get; set; }
        public required CardType CardType { get; set; }

        public virtual Deck Deck { get; set; } = null!;
    }
}
