namespace _09.Models;

/// <summary>
/// Represents an arbitrary position in a 2D grid.
/// </summary>
/// <param name="X">The row value.</param>
/// <param name="Y">The column value.</param>
internal record struct Position(int X, int Y);