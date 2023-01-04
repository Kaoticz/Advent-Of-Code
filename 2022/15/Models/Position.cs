namespace _15.Models;

/// <summary>
/// Represents an arbitrary coordinate in a 2D grid.
/// </summary>
/// <param name="X">The column coordinate.</param>
/// <param name="Y">The row coordinate.</param>
internal sealed record Position(int X, int Y);