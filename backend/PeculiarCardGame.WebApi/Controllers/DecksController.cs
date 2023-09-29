﻿using Microsoft.AspNetCore.Authorization;
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
            return CreatedAtAction(nameof(GetDeck), new { id = deck.Id }, new GetDeckResponse
            {
                Id = deck.Id,
                Name = deck.Name,
                Description = deck.Description
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<GetDeckResponse> GetDeck(int id)
        {
            var deck = _deckManagementService.GetDeck(id);
            if (deck is null)
                return NotFound();
            return Ok(new GetDeckResponse
            {
                Id = deck.Id,
                Name = deck.Name,
                Description = deck.Description
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<GetDeckResponse>> GetAllDecks()
        {
            var decks = _deckManagementService.GetAllDecks();
            return Ok(decks.ConvertAll(x => new GetDeckResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }));
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public ActionResult<List<GetDeckResponse>> FindDecks([FromQuery] string query)
        {
            var decks = _deckManagementService.FindDecks(query);
            return Ok(decks.ConvertAll(x => new GetDeckResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }));
        }

        [HttpPatch("{id}")]
        public ActionResult<GetDeckResponse> UpdateDeck(int id, UpdateDeckRequest request)
        {
            var deck = _deckManagementService.UpdateDeck(id, request.NameUpdate, request.DescriptionUpdate);
            if (deck is null)
                return NotFound();
            return Ok(new GetDeckResponse
            {
                Id = deck.Id,
                Name = deck.Name,
                Description = deck.Description
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
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
            return CreatedAtAction(nameof(CardsController.GetCard), new { id = card.Id }, new GetCardResponse
            {
                Id = card.Id,
                Text = card.Text,
                CardType = card.CardType
            });
        }

        [HttpGet("{deckId}/cards/search")]
        [AllowAnonymous]
        public ActionResult<List<GetCardResponse>> FindCards(int deckId, [FromQuery] string query)
        {
            var cards = _deckManagementService.FindCards(deckId, query);
            return Ok(cards);
        }

        #endregion
    }
}
