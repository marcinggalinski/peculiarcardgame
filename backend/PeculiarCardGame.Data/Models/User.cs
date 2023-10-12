namespace PeculiarCardGame.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string DisplayedName { get; set; }
        public required string PasswordHash { get; set; }

        public virtual ICollection<Deck>? Decks { get; set; }
    }
}
