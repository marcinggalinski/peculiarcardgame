using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.WebApi.Infrastructure.Authentication;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;

namespace PeculiarCardGame.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
    public class DecksController : ControllerBase
    {
        private readonly IDeckManagementService _deckManagementService;

        public DecksController(IDeckManagementService deckManagementService)
        {
            _deckManagementService = deckManagementService;
        }

        #region decks

        [HttpPost]
        public ActionResult<GetDeckResponse> AddDeck(AddDeckRequest request)
        {
            var deck = _deckManagementService.AddDeck(request.Name, request.Description);
            return CreatedAtAction(nameof(GetDeck), new { id = deck.Id }, GetDeckResponse.FromDeck(deck));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<GetDeckResponse> GetDeck(int id)
        {
            var deck = _deckManagementService.GetDeck(id);
            if (deck is null)
                return NotFound();
            return Ok(GetDeckResponse.FromDeck(deck));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<GetDeckResponse>> GetAllDecks()
        {
            var decks = _deckManagementService.GetAllDecks();
            return Ok(decks.ConvertAll(GetDeckResponse.FromDeck));
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public ActionResult<List<GetDeckResponse>> SearchDecks([FromQuery] string? query)
        {
            var decks = _deckManagementService.SearchDecks(query);
            return Ok(decks.ConvertAll(GetDeckResponse.FromDeck));
        }

        [HttpPatch("{id}")]
        public ActionResult<GetDeckResponse> UpdateDeck(int id, UpdateDeckRequest request)
        {
            var deck = _deckManagementService.UpdateDeck(id, request.NameUpdate, request.DescriptionUpdate);
            if (deck is null)
                return NotFound();
            return Ok(GetDeckResponse.FromDeck(deck));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDeck(int id)
        {
            var isDeleted = _deckManagementService.DeleteDeck(id);
            return isDeleted ? Ok() : NotFound();
        }

        #endregion

        #region cards

        [HttpPost("{deckId}/cards")]
        public ActionResult<GetCardResponse> AddCard(int deckId, AddCardRequest request)
        {
            var card = _deckManagementService.AddCard(deckId, request.Text, request.CardType);
            if (card is null)
                return NotFound();
            return CreatedAtAction(nameof(CardsController.GetCard), "cards", new { id = card.Id }, GetCardResponse.FromCard(card));
        }

        [HttpGet("{deckId}/cards")]
        [AllowAnonymous]
        public ActionResult<List<GetCardResponse>> GetCards(int deckId)
        {
            var cards = _deckManagementService.GetAllCards(deckId);
            if (cards is null)
                return NotFound();
            return Ok(cards.ConvertAll(GetCardResponse.FromCard));
        }

        [HttpGet("{deckId}/cards/search")]
        [AllowAnonymous]
        public ActionResult<List<GetCardResponse>> SearchCards(int deckId, [FromQuery] string? query)
        {
            var cards = _deckManagementService.SearchCards(deckId, query);
            if (cards is null)
                return NotFound();
            return Ok(cards.ConvertAll(GetCardResponse.FromCard));
        }

        #endregion
    }
}
