using PeculiarCardGame.Shared;

namespace PeculiarCardGame.WebApi.Models.Requests
{
    public class AddCardRequest
    {
        public string Text { get; set; }
        public CardType CardType { get; set; }
    }
}
