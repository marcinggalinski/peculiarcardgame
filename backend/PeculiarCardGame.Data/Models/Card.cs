﻿namespace PeculiarCardGame.Data.Models
{
    public class Card
    {
        public int Id { get; set; }
        public required int DeckId { get; set; }
        public required string Text { get; set; }
        public CardType CardType { get; set; }

        public virtual Deck? Deck { get; set; }
    }

    
}
