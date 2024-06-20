using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.WebApi.Models.Responses
{
    public class GetDeckResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Author { get; set; }
        public required int AuthorId { get; set; }
        public required int BlackCardCount { get; set; }
        public required int WhiteCardCount { get; set; }

        public static GetDeckResponse FromDeck(Deck deck)
        {
            return new GetDeckResponse
            {
                Id = deck.Id,
                Name = deck.Name,
                Description = deck.Description,
                Author = deck.Author.DisplayedName,
                AuthorId = deck.AuthorId,
                BlackCardCount = deck.Cards.Count(x => x.CardType is Shared.CardType.Black),
                WhiteCardCount = deck.Cards.Count(x => x.CardType is Shared.CardType.White)
            };
        }
    }
}
