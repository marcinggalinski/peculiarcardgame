namespace PeculiarCardGame.Data.Models
{
    public class Card
    {
        public int Id { get; set; }
        public int DeckId { get; set; }
        public string Text { get; set; }
        public CardType CardType { get; set; }

        public virtual Deck Deck { get; set; }
    }

    public enum CardType
    {
        Black,
        White
    }
}
