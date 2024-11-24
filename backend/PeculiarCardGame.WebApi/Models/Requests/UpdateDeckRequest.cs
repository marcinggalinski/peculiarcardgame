using System.ComponentModel.DataAnnotations;
using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.WebApi.Models.Requests
{
    public class UpdateDeckRequest
    {
        [MaxLength(Deck.MaxNameLength)]
        public string? NameUpdate { get; set; }
        [MaxLength(Deck.MaxDescriptionLength)]
        public string? DescriptionUpdate { get; set; }
    }
}
