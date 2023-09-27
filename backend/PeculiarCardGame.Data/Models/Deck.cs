namespace PeculiarCardGame.Data.Models
{
    public class Deck
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual User Author { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
    }
}
