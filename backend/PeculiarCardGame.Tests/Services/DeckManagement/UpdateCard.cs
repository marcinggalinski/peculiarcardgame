﻿using FluentAssertions;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Services;
using PeculiarCardGame.Shared;
using Service = PeculiarCardGame.Services.DeckManagement.DeckManagementService;

namespace PeculiarCardGame.Tests.Services.DeckManagement
{
    public class UpdateCard
    {
        private const string NewText = "new";

        private readonly string _tooLongText = new string('x', Card.MaxTextLength + 1);

        private readonly Deck _deck;
        private readonly Card _card;
        private readonly Card _anotherCard;

        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _emptyRequestContext;
        private readonly RequestContext _authorFilledRequestContext;
        private readonly RequestContext _notAuthorFilledRequestContext;

        public UpdateCard()
        {
            const int DeckId = 1;
            const int AuthorId = 1;
            const int NotAuthorId = 2;
            const int CardId = 1;
            const int AnotherCardId = 2;
            const string DeckName = "test";
            const string DeckDescription = "test";
            const string AuthorUsername = "test";
            const string DisplayedName = "test";
            const string NotAuthorUsername = "test";
            const string CardText = "test";
            const CardType CardType = CardType.Black;

            _deck = new Deck
            {
                Id = DeckId,
                AuthorId = AuthorId,
                Name = DeckName,
                Description = DeckDescription
            };
            _card = new Card
            {
                Id = CardId,
                DeckId = DeckId,
                Text = CardText,
                CardType = CardType,
            };
            _anotherCard = new Card
            {
                Id = AnotherCardId,
                DeckId = DeckId,
                Text = CardText,
                CardType = CardType,
            };

            _dbContext = TestHelpers.GetDbContext();

            _emptyRequestContext = new RequestContext();
            _authorFilledRequestContext = new RequestContext();
            _authorFilledRequestContext.SetOnce(new User
            {
                Id = AuthorId,
                Username = AuthorUsername,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
            _notAuthorFilledRequestContext = new RequestContext();
            _notAuthorFilledRequestContext.SetOnce(new User
            {
                Id = NotAuthorId,
                Username = NotAuthorUsername,
                DisplayedName = DisplayedName,
                PasswordHash = ""
            });
        }

        [Fact]
        public void EmptyRequestContext_ShouldThrowInvalidOperationException()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _emptyRequestContext);

            var action = () => service.UpdateCard(_card.Id, NewText);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EmptyRequestContext_ShouldNotUpdateCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _emptyRequestContext);

            try
            {
                service.UpdateCard(_card.Id, NewText);
            }
            catch (InvalidOperationException) { }
            var card = _dbContext.Cards.Single(x => x.Id == _card.Id);

            card.Id.Should().Be(_card.Id);
            card.DeckId.Should().Be(_card.DeckId);
            card.Text.Should().Be(_card.Text);
            card.CardType.Should().Be(_card.CardType);
        }

        [Fact]
        public void NotExistingCardId_ShouldReturnErrorTypeNotFound()
        {
            _dbContext.SetupTest(_deck);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var result = service.UpdateCard(_card.Id, NewText);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void NotExistingCardId_ShouldNotUpdateCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.UpdateCard(_anotherCard.Id, NewText);
            var card = _dbContext.Cards.Single(x => x.Id == _card.Id);

            card.Id.Should().Be(_card.Id);
            card.DeckId.Should().Be(_card.DeckId);
            card.Text.Should().Be(_card.Text);
            card.CardType.Should().Be(_card.CardType);
        }

        [Fact]
        public void NotAuthor_ShouldReturnErrorTypeUnauthorized()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            var result = service.UpdateCard(_card.Id, NewText);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.Unauthorized);
        }

        [Fact]
        public void NotAuthor_ShouldNotUpdateCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _notAuthorFilledRequestContext);

            service.UpdateCard(_card.Id, NewText);
            var card = _dbContext.Cards.Single(x => x.Id == _card.Id);

            card.Id.Should().Be(_card.Id);
            card.DeckId.Should().Be(_card.DeckId);
            card.Text.Should().Be(_card.Text);
            card.CardType.Should().Be(_card.CardType);
        }

        [Fact]
        public void TooLongText_ShouldReturnErrorTypeConstraintsNotMet()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var result = service.UpdateCard(_card.Id, _tooLongText);

            result.Should().BeLeft();
            result.Left.Should().Be(ErrorType.ConstraintsNotMet);
        }

        [Fact]
        public void TooLongText_ShouldNotUpdateCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.UpdateCard(_card.Id, _tooLongText);
            var card = _dbContext.Cards.Single(x => x.Id == _card.Id);

            card.Id.Should().Be(_card.Id);
            card.DeckId.Should().Be(_card.DeckId);
            card.Text.Should().Be(_card.Text);
            card.CardType.Should().Be(_card.CardType);
        }

        [Fact]
        public void ExistingCardId_ShouldReturnUpdatedCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            var result = service.UpdateCard(_card.Id, NewText);

            result.Should().BeRight();
            result.Right.Id.Should().Be(_card.Id);
            result.Right.DeckId.Should().Be(_card.DeckId);
            result.Right.Text.Should().Be(NewText);
        }

        [Fact]
        public void ExistingCardId_ShouldUpdateCard()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.UpdateCard(_card.Id, NewText);
            var card = _dbContext.Cards.Single(x => x.Id == _card.Id);

            card.Id.Should().Be(_card.Id);
            card.DeckId.Should().Be(_card.DeckId);
            card.Text.Should().Be(NewText);
        }

        [Fact]
        public void ExistingCardId_ShouldNotChangeCardCount()
        {
            _dbContext.SetupTest(_deck);
            _dbContext.SetupTest(_card);
            _dbContext.SetupTest(_anotherCard);
            var cardCountBefore = _dbContext.Cards.Count();
            var service = new Service(_dbContext, _authorFilledRequestContext);

            service.UpdateCard(_card.Id, NewText);

            _dbContext.Cards.Should().HaveCount(cardCountBefore);
        }
    }
}
