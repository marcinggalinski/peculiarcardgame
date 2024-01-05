using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.WebApi.Models.Responses
{
    public class GetDeckResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        public static GetDeckResponse FromDeck(Deck deck)
        {
            return new GetDeckResponse
            {
                Id = deck.Id,
                Name = deck.Name,
                Description = deck.Description
            };
        }
    }
}
