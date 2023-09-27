namespace PeculiarCardGame.WebApi.Models.Responses
{
    public class GetCardResponse
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public CardType CardType { get; set; }
    }
}
