namespace _15.Models;

/// <summary>
/// Represents a sensor.
/// </summary>
/// <param name="Position">The position of this sensor.</param>
/// <param name="BeaconPosition">The position of this sensor's beacon.</param>
internal sealed record Sensor(Position Position, Position BeaconPosition)
{
    /// <summary>
    /// Defines the distance between this sensor and its beacon.
    /// </summary>
    public int BeaconDistance { get; } = ManhattanDistance(Position, BeaconPosition);

    /// <summary>
    /// Determines whether the range of this sensor covers the specified <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns><see langword="true"/> if the position is covered by this sensor, <see langword="false"/> otherwise.</returns>
    public bool CanReach(Position position)
        => BeaconDistance >= ManhattanDistance(Position, position);

    /// <summary>
    /// Gets all positions that are immediately out of range for this sensor.
    /// </summary>
    /// <returns>The positions bordering the range of this sensor.</returns>
    public IEnumerable<Position> GetUnreachablePositions()
    {
        var distance = ManhattanDistance(Position, BeaconPosition) + 1;

        yield return new(Position.X - distance, Position.Y);    // Left
        yield return new(Position.X + distance, Position.Y);    // Right
        yield return new(Position.X, Position.Y - distance);    // Top
        yield return new(Position.X, Position.Y + distance);    // Bottom

        // Diagonals
        for (var counter = 1; counter <= distance; counter++)
        {
            yield return new(Position.X - distance + counter, Position.Y - counter);    // Top Left
            yield return new(Position.X - distance + counter, Position.Y + counter);    // Bottom Left
            yield return new(Position.X + distance - counter, Position.Y - counter);    // Top Right
            yield return new(Position.X + distance - counter, Position.Y + counter);    // Bottom Right
        }
    }

    /// <summary>
    /// Calculates the Manhattan distance between two points.
    /// </summary>
    /// <param name="start">The starting position.</param>
    /// <param name="end">The end position.</param>
    /// <returns>The Manhattan distance between <paramref name="start"/> and <paramref name="end"/>.</returns>
    private static int ManhattanDistance(Position start, Position end)
    {
        var distanceX = Math.Abs(Math.Max(start.X, end.X) - Math.Min(start.X, end.X));
        var distanceY = Math.Abs(Math.Max(start.Y, end.Y) - Math.Min(start.Y, end.Y));

        return distanceX + distanceY;
    }
}