using PeculiarCardGame.Shared;

namespace PeculiarCardGame.WebApi.Models.Requests
{
    public class UpdateCardRequest
    {
        public string? TextUpdate { get; set; }
        public CardType? CardTypeUpdate { get; set; }
    }
}
