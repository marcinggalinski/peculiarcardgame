using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.Services.DeckManagement
{
    public sealed class DeckManagementService : IDeckManagementService
    {
        private readonly PeculiarCardGameDbContext _dbContext;

        public DeckManagementService(PeculiarCardGameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region decks

        public Deck AddDeck(string name, string description)
        {
            throw new NotImplementedException();
        }

        public Deck? GetDeck(int id)
        {
            throw new NotImplementedException();
        }

        public List<Deck> GetAllDecks()
        {
            throw new NotImplementedException();
        }

        public List<Deck> FindDecks(string query)
        {
            throw new NotImplementedException();
        }

        public Deck? UpdateDeck(int id, string? nameUpdate, string? descriptionUpdate)
        {
            throw new NotImplementedException();
        }

        public void DeleteDeck(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region cards

        public Card? AddCard(int deckId, string text, CardType type)
        {
            throw new NotImplementedException();
        }

        public List<Card> GetAllCards(int deckId)
        {
            throw new NotImplementedException();
        }

        public Card? GetCard(int id)
        {
            throw new NotImplementedException();
        }

        public List<Card> FindCards(int deckId, string query)
        {
            throw new NotImplementedException();
        }

        public Card? UpdateCard(int id, string? textUpdate, CardType? typeUpdate)
        {
            throw new NotImplementedException();
        }

        public void DeleteCard(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
