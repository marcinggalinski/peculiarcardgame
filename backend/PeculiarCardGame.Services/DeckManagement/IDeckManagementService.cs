using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.Services.DeckManagement
{
    public interface IDeckManagementService
    {
        /// <remarks>Requires request context to be set.</remarks>
        Deck AddDeck(string name, string? description);
        Deck? GetDeck(int id);
        List<Deck> GetAllDecks();
        List<Deck> FindDecks(string? query);
        /// <remarks>Requires request context to be set.</remarks>
        Deck? UpdateDeck(int id, string? nameUpdate, string? descriptionUpdate);
        /// <remarks>Requires request context to be set.</remarks>
        bool DeleteDeck(int id);

        /// <remarks>Requires request context to be set.</remarks>
        Card? AddCard(int deckId, string text, CardType type);
        Card? GetCard(int id);
        List<Card> GetAllCards(int deckId);
        List<Card> FindCards(int deckId, string? query);
        /// <remarks>Requires request context to be set.</remarks>
        Card? UpdateCard(int id, string? textUpdate, CardType? typeUpdate);
        /// <remarks>Requires request context to be set.</remarks>
        bool DeleteCard(int id);
    }
}
