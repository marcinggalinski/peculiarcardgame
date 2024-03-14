using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.WebApi.Infrastructure.Authentication;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PeculiarCardGame.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
    [SwaggerTag("Exposes endpoints for card-specific deck-related operations.")]
    public class CardsController : ControllerBase
    {
        private readonly IDeckManagementService _deckManagementService;

        public CardsController(IDeckManagementService deckManagementService)
        {
            _deckManagementService = deckManagementService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [SwaggerOperation("Gets specified card.", "Doesn't require any authentication data.")]
        [SwaggerResponse(200, "Card found", typeof(GetDeckResponse))]
        [SwaggerResponse(404, "Card not found")]
        public ActionResult<GetCardResponse> GetCard(int id)
        {
            var card = _deckManagementService.GetCard(id);
            if (card is null)
                return NotFound();
            return Ok(GetCardResponse.FromCard(card));
        }

        [HttpPatch("{id}")]
        [SwaggerOperation("Updates specified card.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Doesn't allow modyfying cards from other users' decks.")]
        [SwaggerResponse(200, "Card updated", typeof(GetDeckResponse))]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "Card not found")]
        public ActionResult<GetCardResponse> UpdateCard(int id, UpdateCardRequest request)
        {
            var card = _deckManagementService.UpdateCard(id, request.TextUpdate, request.CardTypeUpdate);
            if (card is null)
                return NotFound();
            return Ok(GetCardResponse.FromCard(card));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Deletes specified card.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Doesn't allow deleting cards from other users' decks.")]
        [SwaggerResponse(200, "Card deleted", typeof(GetDeckResponse))]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "Card not found")]
        public IActionResult DeleteCard(int id)
        {
            var isDeleted = _deckManagementService.DeleteCard(id);
            return isDeleted ? Ok() : NotFound();
        }
    }
}
