using Microsoft.EntityFrameworkCore;
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

        public Either<ErrorType, Deck> AddDeck(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(AddDeck)} can only be called by an authenticated user.");

            name = name.Trim();
            description = description?.Trim() ?? string.Empty;

            if (name.Length > Deck.MaxNameLength || description.Length > Deck.MaxDescriptionLength)
                return ErrorType.ConstraintsNotMet;

            var deck = _dbContext.Decks.Add(new Deck
            {
                Name = name,
                Description = description,
                AuthorId = _requestContext.CallingUser.Id
            }).Entity;
            _dbContext.SaveChanges();

            return deck;
        }

        public Either<ErrorType, Deck> GetDeck(int id)
        {
            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == id);
            if (deck is null)
                return ErrorType.NotFound;
            return deck;
        }

        public List<Deck> SearchDecks(string? filter = null, int? authorId = null)
        {
            filter ??= string.Empty;

            IQueryable<Deck> query = _dbContext.Decks;
            if (filter != string.Empty)
                query = query.Where(x => x.Name.Contains(filter) || x.Description.Contains(filter));
            if (authorId is not null)
                query = query.Where(x => x.AuthorId == authorId);

            var decks = query.ToList();
            return decks;
        }

        public Either<ErrorType, Deck> UpdateDeck(int id, string? nameUpdate, string? descriptionUpdate)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateDeck)} can only be called by an authenticated user.");

            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == id);
            if (deck is null)
                return ErrorType.NotFound;

            if (_requestContext.CallingUser.Id != deck.AuthorId)
                return ErrorType.Unauthorized;

            nameUpdate = nameUpdate?.Trim();
            descriptionUpdate = descriptionUpdate?.Trim();

            if (nameUpdate is not null && nameUpdate.Length > Deck.MaxNameLength
                || descriptionUpdate is not null && descriptionUpdate.Length > Deck.MaxDescriptionLength)
                return ErrorType.ConstraintsNotMet;

            if (!string.IsNullOrEmpty(nameUpdate))
                deck.Name = nameUpdate;
            if (!string.IsNullOrEmpty(descriptionUpdate))
                deck.Description = descriptionUpdate;

            deck = _dbContext.Decks.Update(deck).Entity;
            _dbContext.SaveChanges();

            return deck;
        }

        public ErrorType? DeleteDeck(int id)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(DeleteDeck)} can only be called by an authenticated user.");

            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == id);
            if (deck is null)
                return ErrorType.NotFound;

            if (_requestContext.CallingUser.Id != deck.AuthorId)
                return ErrorType.Unauthorized;

            _dbContext.Decks.Remove(deck);
            _dbContext.SaveChanges();

            return null;
        }

        #endregion

        #region cards

        public Either<ErrorType, Card> AddCard(int deckId, string text, CardType type)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(AddCard)} can only be called by an authenticated user.");

            var deck = _dbContext.Decks.SingleOrDefault(x => x.Id == deckId);
            if (deck is null)
                return ErrorType.NotFound;

            if (_requestContext.CallingUser.Id != deck.AuthorId)
                return ErrorType.Unauthorized;

            text = text.Trim();
            if (text.Length > Card.MaxTextLength)
                return ErrorType.ConstraintsNotMet;

            var card = _dbContext.Cards.Add(new Card
            {
                DeckId = deckId,
                Text = text,
                CardType = type
            }).Entity;
            _dbContext.SaveChanges();

            return card;
        }

        public Either<ErrorType, Card> GetCard(int id)
        {
            var card = _dbContext.Cards.SingleOrDefault(x => x.Id == id);
            if (card is null)
                return ErrorType.NotFound;
            return card;
        }

        public Either<ErrorType, List<Card>> SearchCards(int deckId, string? filter = null)
        {
            filter ??= string.Empty;

            var deck = _dbContext.Decks.Include(x => x.Cards).SingleOrDefault(x => x.Id == deckId);
            if (deck is null)
                return ErrorType.NotFound;

            IEnumerable<Card> query = deck.Cards;
            if (filter != "")
                query = query.Where(x => x.Text.Contains(filter));

            var cards = query.ToList();
            return cards;
        }

        public Either<ErrorType, Card> UpdateCard(int id, string textUpdate)
        {
            if (string.IsNullOrWhiteSpace(textUpdate))
                throw new ArgumentNullException(nameof(textUpdate));
            
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(UpdateCard)} can only be called by an authenticated user.");

            var card = _dbContext.Cards.Include(x => x.Deck).SingleOrDefault(x => x.Id == id);
            if (card is null)
                return ErrorType.NotFound;

            if (_requestContext.CallingUser.Id != card.Deck.AuthorId)
                return ErrorType.Unauthorized;

            textUpdate = textUpdate.Trim();
            if (textUpdate.Length > Card.MaxTextLength)
                return ErrorType.ConstraintsNotMet;

            if (!string.IsNullOrEmpty(textUpdate))
                card.Text = textUpdate;

            card = _dbContext.Cards.Update(card).Entity;
            _dbContext.SaveChanges();

            return card;
        }

        public ErrorType? DeleteCard(int id)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(DeleteCard)} can only be called by an authenticated user.");

            var card = _dbContext.Cards.Include(x => x.Deck).SingleOrDefault(x => x.Id == id);
            if (card is null)
                return ErrorType.NotFound;

            if (_requestContext.CallingUser.Id != card.Deck.AuthorId)
                return ErrorType.Unauthorized;

            _dbContext.Cards.Remove(card);
            _dbContext.SaveChanges();

            return null;
        }

        #endregion
    }
}
