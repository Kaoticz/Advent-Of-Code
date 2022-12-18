using _14.Enums;

namespace _14.Models;

/// <summary>
/// Represents an arbitrary position in a 2D grid.
/// </summary>
/// <param name="Content">The content of this position.</param>
/// <param name="X">The row position.</param>
/// <param name="Y">The column position.</param>
internal sealed record Position(Content Content, int X, int Y);