namespace _14.Enums;

/// <summary>
/// Represents the content present in a position.
/// </summary>
internal enum Content
{
    /// <summary>
    /// Air. Does not move, but also doesn't hinder movement.
    /// </summary>
    Air,

    /// <summary>
    /// Rock. Does not move, but hinders movement.
    /// </summary>
    Rock,

    /// <summary>
    /// Sand. Does move, and also hinders movement.
    /// </summary>
    Sand
}