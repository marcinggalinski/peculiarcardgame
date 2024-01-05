using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.WebApi.Models.Responses
{
    public class GetCardResponse
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public CardType CardType { get; set; }

        public static GetCardResponse FromCard(Card card)
        {
            return new GetCardResponse
            {
                Id = card.Id,
                Text = card.Text,
                CardType = card.CardType
            };
        }
    }
}
