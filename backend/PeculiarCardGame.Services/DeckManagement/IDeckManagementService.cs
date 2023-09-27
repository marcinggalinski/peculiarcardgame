using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.Services.DeckManagement
{
    public interface IDeckManagementService
    {
        Deck AddDeck(string name, string description);
        Deck? GetDeck(int id);
        List<Deck> GetAllDecks();
        List<Deck> FindDecks(string query);
        Deck? UpdateDeck(int id, string? nameUpdate, string? descriptionUpdate);
        bool DeleteDeck(int id);

        Card? AddCard(int deckId, string text, CardType type);
        Card? GetCard(int id);
        List<Card> GetAllCards(int deckId);
        List<Card> FindCards(int deckId, string query);
        Card? UpdateCard(int id, string? textUpdate, CardType? typeUpdate);
        bool DeleteCard(int id);
    }
}
