using System.ComponentModel.DataAnnotations;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Shared;

namespace PeculiarCardGame.WebApi.Models.Requests
{
    public class AddCardRequest
    {
        [MaxLength(Card.MaxTextLength)]
        public required string Text { get; set; }
        public CardType CardType { get; set; }
    }
}
