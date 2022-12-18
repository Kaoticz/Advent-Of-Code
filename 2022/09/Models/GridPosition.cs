namespace _09.Models;

/// <summary>
/// Represents an arbitrary position in a 2D grid.
/// </summary>
/// <param name="X">The column value.</param>
/// <param name="Y">The row value.</param>
internal sealed record struct Position(int X, int Y);