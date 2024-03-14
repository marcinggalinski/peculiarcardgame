using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.WebApi.Infrastructure.Authentication;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PeculiarCardGame.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
    [SwaggerTag("Exposes endpoints for deck-related operations.")]
    public class DeckManagementController : ControllerBase
    {
        private readonly IDeckManagementService _deckManagementService;

        public DeckManagementController(IDeckManagementService deckManagementService)
        {
            _deckManagementService = deckManagementService;
        }

        #region decks

        [HttpPost("decks")]
        [SwaggerOperation("Creates a new deck.", "Requires valid bearer token authentication data to be sent in 'Authorization' header.")]
        [SwaggerResponse(201, "Deck created", typeof(GetDeckResponse))]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        public ActionResult<GetDeckResponse> AddDeck(AddDeckRequest request)
        {
            var deck = _deckManagementService.AddDeck(request.Name, request.Description);
            return CreatedAtAction(nameof(GetDeck), new { id = deck.Id }, GetDeckResponse.FromDeck(deck));
        }

        [HttpGet("decks/{id}")]
        [AllowAnonymous]
        [SwaggerOperation("Gets specified deck.", "Doesn't require any authentication data. Only returns information about the deck, not the cards it consists of.")]
        [SwaggerResponse(200, "Deck found", typeof(GetDeckResponse))]
        [SwaggerResponse(404, "Deck not found")]
        public ActionResult<GetDeckResponse> GetDeck(int id)
        {
            var deck = _deckManagementService.GetDeck(id);
            if (deck is null)
                return NotFound();
            return Ok(GetDeckResponse.FromDeck(deck));
        }

        [HttpGet("decks")]
        [AllowAnonymous]
        [SwaggerOperation("Gets all decks that match given query.", "Doesn't require any authentication data. Only returns information about decks, not the cards they consist of. Does not support paging.")]
        [SwaggerResponse(200, "Decks found", typeof(List<GetDeckResponse>))]
        public ActionResult<List<GetDeckResponse>> GetAllDecks([FromQuery] string? query)
        {
            var decks = string.IsNullOrEmpty(query) ? _deckManagementService.GetAllDecks() : _deckManagementService.SearchDecks(query);
            return Ok(decks.ConvertAll(GetDeckResponse.FromDeck));
        }

        [HttpPatch("decks/{id}")]
        [SwaggerOperation("Updates specified deck.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Only updates basic information about the deck, to add, delete or modify cards the deck consists of use other endpoints. Doesn't allow modyfying other users' decks.")]
        [SwaggerResponse(200, "Deck updated", typeof(GetDeckResponse))]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "Deck not found")]
        public ActionResult<GetDeckResponse> UpdateDeck(int id, UpdateDeckRequest request)
        {
            var deck = _deckManagementService.UpdateDeck(id, request.NameUpdate, request.DescriptionUpdate);
            if (deck is null)
                return NotFound();
            return Ok(GetDeckResponse.FromDeck(deck));
        }

        [HttpDelete("decks/{id}")]
        [SwaggerOperation("Deletes specified deck.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Doesn't allow deleting other users' decks.")]
        [SwaggerResponse(200, "Deck deleted")]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "Deck not found")]
        public IActionResult DeleteDeck(int id)
        {
            var isDeleted = _deckManagementService.DeleteDeck(id);
            return isDeleted ? Ok() : NotFound();
        }

        #endregion

        #region cards

        [HttpPost("decks/{deckId}/cards")]
        [SwaggerOperation("Adds a new card to the specified deck.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Doesn't allow adding cards to other users' decks.")]
        [SwaggerResponse(201, "Card created", typeof(GetCardResponse))]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "Deck not found")]
        public ActionResult<GetCardResponse> AddCard(int deckId, AddCardRequest request)
        {
            var card = _deckManagementService.AddCard(deckId, request.Text, request.CardType);
            if (card is null)
                return NotFound();
            return CreatedAtAction(nameof(GetCard), new { id = card.Id }, GetCardResponse.FromCard(card));
        }

        [AllowAnonymous]
        [HttpGet("cards/{id}")]
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

        [HttpGet("decks/{deckId}/cards")]
        [AllowAnonymous]
        [SwaggerOperation("Gets all cards belonging to the specified deck that match given query.", "Doesn't require any authentication data. Does not support paging.")]
        [SwaggerResponse(200, "Cards found", typeof(List<GetCardResponse>))]
        [SwaggerResponse(404, "Deck not found")]
        public ActionResult<List<GetCardResponse>> GetCards(int deckId, [FromQuery] string? query)
        {
            var cards = string.IsNullOrEmpty(query) ? _deckManagementService.GetAllCards(deckId) : _deckManagementService.SearchCards(deckId, query);
            if (cards is null)
                return NotFound();
            return Ok(cards.ConvertAll(GetCardResponse.FromCard));
        }

        [HttpPatch("cards/{id}")]
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

        [HttpDelete("cards/{id}")]
        [SwaggerOperation("Deletes specified card.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Doesn't allow deleting cards from other users' decks.")]
        [SwaggerResponse(200, "Card deleted")]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "Card not found")]
        public IActionResult DeleteCard(int id)
        {
            var isDeleted = _deckManagementService.DeleteCard(id);
            return isDeleted ? Ok() : NotFound();
        }

        #endregion
    }
}
