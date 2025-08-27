using System.ComponentModel.DataAnnotations;

namespace PeculiarCardGame.Data.Models;

public class TokenInfo
{
    [Key]
    [MaxLength(32)]
    public required string Token { get; init; }
    public bool IsRevoked { get; set; }
    public required DateTime ExpirationDateUtc { get; init; }
    public required int UserId { get; init; }

    public virtual User User { get; set; } = null!;
}