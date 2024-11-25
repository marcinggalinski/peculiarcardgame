using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Shared;

namespace PeculiarCardGame.Services.DeckManagement
{
    public interface IDeckManagementService
    {
        /// <remarks>Requires request context to be set.</remarks>
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.ConstraintsNotMet"/>,
        /// </returns>
        Either<ErrorType, Deck> AddDeck(string name, string? description);
        
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.NotFound"/>,
        /// </returns>
        Either<ErrorType, Deck> GetDeck(int id);
        
        List<Deck> GetAllDecks();
        List<Deck> SearchDecks(string? query);
        
        /// <remarks>Requires request context to be set.</remarks>
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.ConstraintsNotMet"/>,
        /// <see cref="ErrorType.NotFound"/>,
        /// <see cref="ErrorType.Unauthorized"/>
        /// </returns>
        Either<ErrorType, Deck> UpdateDeck(int id, string? nameUpdate, string? descriptionUpdate);
        
        /// <remarks>Requires request context to be set.</remarks>
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.NotFound"/>,
        /// <see cref="ErrorType.Unauthorized"/>
        /// </returns>
        ErrorType? DeleteDeck(int id);

        /// <remarks>Requires request context to be set.</remarks>
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.ConstraintsNotMet"/>,
        /// <see cref="ErrorType.NotFound"/>,
        /// <see cref="ErrorType.Unauthorized"/>
        /// </returns>
        Either<ErrorType, Card> AddCard(int deckId, string text, CardType type);
        
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.NotFound"/>
        /// </returns>
        Either<ErrorType, Card> GetCard(int id);
        
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.NotFound"/>
        /// </returns>
        Either<ErrorType, List<Card>> GetAllCards(int deckId);
        
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.NotFound"/>
        /// </returns>
        Either<ErrorType, List<Card>> SearchCards(int deckId, string? query);
        
        /// <remarks>Requires request context to be set.</remarks>
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.ConstraintsNotMet"/>,
        /// <see cref="ErrorType.NotFound"/>,
        /// <see cref="ErrorType.Unauthorized"/>
        /// </returns>
        Either<ErrorType, Card> UpdateCard(int id, string textUpdate);
        
        /// <remarks>Requires request context to be set.</remarks>
        /// <returns>
        /// Possible <see cref="ErrorType"/>s:
        /// <see cref="ErrorType.NotFound"/>,
        /// <see cref="ErrorType.Unauthorized"/>
        /// </returns>
        ErrorType? DeleteCard(int id);
    }
}
