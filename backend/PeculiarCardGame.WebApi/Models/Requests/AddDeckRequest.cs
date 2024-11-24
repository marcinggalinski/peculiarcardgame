using System.ComponentModel.DataAnnotations;
using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.WebApi.Models.Requests
{
    public class AddDeckRequest
    {
        [MaxLength(Deck.MaxNameLength)]
        public required string Name { get; set; }
        [MaxLength(Deck.MaxDescriptionLength)]
        public string? Description { get; set; }
    }
}
