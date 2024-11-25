using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class AddDeck
    {
        private const int UserId = 1;
        private const string Username = "test";
        private const string DisplayedName = "test";

        private const string DeckName = "test";
        private const string DeckDescription = "test";

        private readonly string _tooLongDeckName = new string('x', Deck.MaxNameLength + 1);
        private readonly string _tooLongDescription = new string('x', Deck.MaxDescriptionLength + 1);

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _filledRequestContext;

        public AddDeck()
        {
            _dbContext = TestHelpers.GetDbContext();

            _emptyRequestContext = new RequestContext();
            _filledRequestContext = new RequestContext();
            _filledRequestContext.SetOnce(new User
            {
                Id = UserId,
                Username = Username,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
        }

        [Fact]
        public void NullName_ShouldThrowArgumentNullException()
        {
            var service = new Service(_dbContext, _filledRequestContext);

#pragma warning disable CS8625
            var action = () => service.AddDeck(null, DeckDescription);
#pragma warning restore CS8625

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NullName_ShouldNotAddDeck()
        {
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            try
            {
#pragma warning disable CS8625
                service.AddDeck(null, DeckDescription);
#pragma warning restore CS8625
            }
            catch (ArgumentNullException) { }

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            var service = new Service(_dbContext, _emptyRequestContext);

            var action = () => service.AddDeck(DeckName, DeckDescription);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldNotAddDeck()
        {
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
                service.AddDeck(DeckName, DeckDescription);
            }
            catch (InvalidOperationException) { }

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }

        [Fact]
        public void TooLongDeckName_ShouldReturnErrorTypeConstraintsNotMet()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var result = service.AddDeck(_tooLongDeckName, DeckDescription);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.ConstraintsNotMet);
        }

        [Fact]
        public void TooLongDeckName_ShouldNotAddDeck()
        {
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.AddDeck(_tooLongDeckName, DeckDescription);

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }

        [Fact]
        public void TooLongDescription_ShouldReturnErrorTypeConstraintsNotMet()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var result = service.AddDeck(DeckName, _tooLongDescription);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.ConstraintsNotMet);
        }

        [Fact]
        public void TooLongDescription_ShouldNotAddDeck()
        {
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.AddDeck(DeckName, _tooLongDescription);

            _dbContext.Decks.Should().HaveCount(deckCountBefore);
        }

        [Fact]
        public void FilledRequestContext_ShouldAddDeck()
        {
            var deckCountBefore = _dbContext.Decks.Count();
            var service = new Service(_dbContext, _filledRequestContext);

            service.AddDeck(DeckName, DeckDescription);

            _dbContext.Decks.Should().HaveCount(deckCountBefore + 1);
        }

        [Fact]
        public void FilledRequestContext_ShouldReturnNewDeck()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var result = service.AddDeck(DeckName, DeckDescription);

            result.Should().BeRight();
            result.Right.Name.Should().Be(DeckName);
            result.Right.Description.Should().Be(DeckDescription);
            result.Right.AuthorId.Should().Be(UserId);
        }

        [Fact]
        public void NullDescription_ShouldAddDeckWithEmptyStringAsDescription()
        {
            var service = new Service(_dbContext, _filledRequestContext);

            var deck = service.AddDeck(DeckName, null);

            deck.Right.Description.Should().Be("");
        }
    }
}
