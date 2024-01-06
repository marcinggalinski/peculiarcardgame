using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.WebApi.Infrastructure.Authentication;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;

namespace PeculiarCardGame.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
    public class CardsController : ControllerBase
    {
        private readonly IDeckManagementService _deckManagementService;

        public CardsController(IDeckManagementService deckManagementService)
        {
            _deckManagementService = deckManagementService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<GetCardResponse> GetCard(int id)
        {
            var card = _deckManagementService.GetCard(id);
            if (card is null)
                return NotFound();
            return Ok(GetCardResponse.FromCard(card));
        }

        [HttpPatch("{id}")]
        public ActionResult<GetCardResponse> UpdateCard(int id, UpdateCardRequest request)
        {
            var card = _deckManagementService.UpdateCard(id, request.TextUpdate, request.CardTypeUpdate);
            if (card is null)
                return NotFound();
            return Ok(GetCardResponse.FromCard(card));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCard(int id)
        {
            var isDeleted = _deckManagementService.DeleteCard(id);
            return isDeleted ? Ok() : NotFound();
        }
    }
}
