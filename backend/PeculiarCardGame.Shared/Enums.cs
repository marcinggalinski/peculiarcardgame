namespace PeculiarCardGame.Shared;

public enum CardType
{
    Black,
    White
}

/// <summary>
/// Defines possible non-success results from services.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Entity was not found.
    /// </summary>
    NotFound,
    /// <summary>
    /// A conflict was detected.
    /// </summary>
    Conflict,
    /// <summary>
    /// Constraints (e.g. max length) were not met.
    /// </summary>
    ConstraintsNotMet,
    /// <summary>
    /// User was unauthorized for the specified action.
    /// </summary>
    Unauthorized
}
