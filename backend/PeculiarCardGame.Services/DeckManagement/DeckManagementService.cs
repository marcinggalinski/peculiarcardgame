using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Shared;

namespace PeculiarCardGame.Services.DeckManagement
{
    public sealed class DeckManagementService : IDeckManagementService
    {
        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public DeckManagementService(PeculiarCardGameDbContext dbContext, RequestContext requestContext)
        {
            _dbContext = dbContext;
            _requestContext = requestContext;
        }

        #region decks

        public Deck AddDeck(string name, string? description)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(AddDeck)} can only be called by an authenticated user.");

            description ??= string.Empty;
            var deck = _dbContext.Decks.Add(new Deck
            {
                Name = name,
                Description = description,
                AuthorId = _requestContext.CallingUser.Id
            }).Entity;
            _dbContext.SaveChanges();

            return deck;
        }

        public Deck? GetDeck(int id)
        {
            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == id);
            return deck;
        }

        public List<Deck> GetAllDecks()
        {
            var decks = _dbContext.Decks.ToList();
            return decks;
        }

        public List<Deck> SearchDecks(string? query)
        {
            query ??= string.Empty;

            var decks = _dbContext.Decks.Where(x => x.Name.Contains(query) || x.Description.Contains(query)).ToList();
            return decks;
        }

        public Deck? UpdateDeck(int id, string? nameUpdate, string? descriptionUpdate)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateDeck)} can only be called by an authenticated user.");

            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == id && x.AuthorId == _requestContext.CallingUser.Id);
            if (deck is null)
                return null;

            if (nameUpdate is not null)
                deck.Name = nameUpdate;
            if (descriptionUpdate is not null)
                deck.Description = descriptionUpdate;

            deck = _dbContext.Decks.Update(deck).Entity;
            _dbContext.SaveChanges();

            return deck;
        }

        public bool DeleteDeck(int id)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(DeleteDeck)} can only be called by an authenticated user.");

            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == id && x.AuthorId == _requestContext.CallingUser.Id);
            if (deck is null)
                return false;

            _dbContext.Decks.Remove(deck);
            _dbContext.SaveChanges();

            return true;
        }

        #endregion

        #region cards

        public Card? AddCard(int deckId, string text, CardType type)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(AddCard)} can only be called by an authenticated user.");

            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == deckId && x.AuthorId == _requestContext.CallingUser.Id);
            if (deck is null)
                return null;

            var card = _dbContext.Cards.Add(new Card
            {
                DeckId = deckId,
                Text = text,
                CardType = type
            }).Entity;
            _dbContext.SaveChanges();

            return card;
        }

        public List<Card> GetAllCards(int deckId)
        {
            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == deckId);
            if (deck is null)
                return new List<Card>();

            var cards = deck.Cards!.ToList();
            return cards;
        }

        public Card? GetCard(int id)
        {
            var card = _dbContext.Cards.SingleOrDefault(x => x.Id == id);
            return card;
        }

        public List<Card>? SearchCards(int deckId, string? query)
        {
            query ??= string.Empty;

            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == deckId);
            if (deck is null)
                return null;

            var cards = deck.Cards!.Where(x => x.Text.Contains(query)).ToList();
            return cards;
        }

        public Card? UpdateCard(int id, string? textUpdate, CardType? typeUpdate)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateCard)} can only be called by an authenticated user.");

            var card = _dbContext.Cards.SingleOrDefault(x => x.Id == id && x.Deck!.AuthorId == _requestContext.CallingUser.Id);
            if (card is null)
                return null;

            if (textUpdate is not null)
                card.Text = textUpdate;
            if (typeUpdate is not null)
                card.CardType = typeUpdate.Value;

            card = _dbContext.Cards.Update(card).Entity;
            _dbContext.SaveChanges();

            return card;
        }

        public bool DeleteCard(int id)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(DeleteCard)} can only be called by an authenticated user.");

            var card = _dbContext.Cards.SingleOrDefault(x => x.Id == id && x.Deck!.AuthorId == _requestContext.CallingUser.Id);
            if (card is null)
                return false;

            _dbContext.Cards.Remove(card);
            _dbContext.SaveChanges();

            return true;
        }

        #endregion
    }
}
