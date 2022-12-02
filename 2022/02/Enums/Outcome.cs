namespace _02.Enums;

/// <summary>
/// Represents the possible outcomes for a game.
/// </summary>
internal enum Outcome
{
    /// <summary>
    /// Defeat. Reaching this outcome grants a score of 0.
    /// </summary>
    Defeat = 0,

    /// <summary>
    /// Draw. Reaching this outcome grants a score of 3.
    /// </summary>
    Draw = 3,

    /// <summary>
    /// Victory. Reaching this outcome grants a score of 6.
    /// </summary>
    Victory = 6
}
