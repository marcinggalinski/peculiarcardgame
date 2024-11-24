using System.ComponentModel.DataAnnotations;
using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.WebApi.Models.Requests
{
    public class UpdateCardRequest
    {
        [MaxLength(Card.MaxTextLength)]
        public string TextUpdate { get; set; } = null!;
    }
}
