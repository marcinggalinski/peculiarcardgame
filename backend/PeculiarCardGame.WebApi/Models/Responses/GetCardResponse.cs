using PeculiarCardGame.Data.Models;
using System.Text.Json.Serialization;

namespace PeculiarCardGame.WebApi.Models.Responses
{
    public class GetCardResponse
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
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
