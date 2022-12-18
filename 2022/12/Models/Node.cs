namespace _12.Models;

/// <summary>
/// Represents a node in a 3D grid.
/// </summary>
/// <param name="Value">The height (Z) coordinate.</param>
/// <param name="X">The X coordinate.</param>
/// <param name="Y">The Y coordinate.</param>
internal sealed record Node(char Value, int X, int Y)
{
    /// <summary>
    /// Determines whether moving from <paramref name="source"/> to <paramref name="destination"/> is possible,
    /// where dropping to infinitely low nodes is allowed.
    /// </summary>
    /// <param name="source">The origin node.</param>
    /// <param name="destination">The destination node.</param>
    /// <returns><see langword="true"/> if the movement is possible, <see langword="false"/> otherwise.</returns>
    public static bool CanMoveDropping(Node source, Node destination)
    {
        var sourceChar = (source.Value is 'S') ? 'a' : (source.Value is 'E') ? 'z' : source.Value;
        var destinationChar = (destination.Value is 'S') ? 'a' : (destination.Value is 'E') ? 'z' : destination.Value;

        return IsCloseBy(source, destination) && destinationChar <= sourceChar + 1;
    }

    /// <summary>
    /// Determines whether moving from <paramref name="source"/> to <paramref name="destination"/> is possible,
    /// where climbing to infinitely high nodes is allowed.
    /// </summary>
    /// <param name="source">The origin node.</param>
    /// <param name="destination">The destination node.</param>
    /// <returns><see langword="true"/> if the movement is possible, <see langword="false"/> otherwise.</returns>
    public static bool CanMoveClimbing(Node source, Node destination)
    {
        var sourceChar = (source.Value is 'S') ? 'a' : (source.Value is 'E') ? 'z' : source.Value;
        var destinationChar = (destination.Value is 'S') ? 'a' : (destination.Value is 'E') ? 'z' : destination.Value;

        return IsCloseBy(source, destination) && destinationChar >= sourceChar - 1;
    }

    /// <summary>
    /// Determines whether <paramref name="source"/> is horizontally or vertically adjacent to <paramref name="destination"/>.
    /// </summary>
    /// <param name="source">The origin node.</param>
    /// <param name="destination">The destination node.</param>
    /// <returns><see langword="true"/> if the nodes are adjacent, <see langword="false"/> otherwise.</returns>
    private static bool IsCloseBy(Node source, Node destination)
    {
        return destination.Y <= source.Y + 1 && destination.Y >= source.Y - 1
            && destination.X <= source.X + 1 && destination.X >= source.X - 1
            && (destination.X == source.X || destination.Y == source.Y);
    }
}