using _15.Models;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace _15;

internal sealed partial class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");
    private static readonly Regex _inputRegex = SensorRegex();
    private const int _minBoundary = 0;
    private const int _maxBoundary = 4_000_000;

    private static async Task Main()
    {
        var sensors = (await File.ReadAllLinesAsync(_inputLocation))
            .Select(x => ParseSensor(x, _inputRegex))
            .ToImmutableArray();

        var distressBeacon = FindDistressBeacon(sensors, new(_minBoundary, _maxBoundary), new(_minBoundary, _maxBoundary));

        Console.WriteLine($"First answer: {CountCoveredPositions(sensors, sensors.Length > 14 ? 2_000_000 : 10)}");
        Console.WriteLine($"Second answer: {(distressBeacon.X * (long)_maxBoundary) + distressBeacon.Y}");
    }

    /// <summary>
    /// Parses a sensor from raw input data.
    /// </summary>
    /// <param name="input">The raw input data.</param>
    /// <param name="inputRegex">The regex to parse the input.</param>
    /// <returns>A parsed <see cref="Sensor"/>.</returns>
    private static Sensor ParseSensor(string input, Regex inputRegex)
    {
        var matches = inputRegex.Matches(input);
        var sensorPosition = new Position(int.Parse(matches[0].ValueSpan), int.Parse(matches[1].ValueSpan));
        var beaconPosition = new Position(int.Parse(matches[2].ValueSpan), int.Parse(matches[3].ValueSpan));

        return new Sensor(sensorPosition, beaconPosition);
    }

    /// <summary>
    /// Counts the amount of positions covered by sensors in the specified <paramref name="rowIndex"/>.
    /// </summary>
    /// <param name="sensors">The sensors in the room.</param>
    /// <param name="rowIndex">The row index to count positions from.</param>
    /// <returns>The amount of positions covered in the specified row coordinate.</returns>
    private static int CountCoveredPositions(IReadOnlyList<Sensor> sensors, int rowIndex)
    {
        var (width, _) = GetRoomDimensions(sensors);

        return Enumerable.Range(width.Start, width.Length)
            .Select(x => new Position(x, rowIndex))
            .Count(newPosition => sensors.Any(y => y.BeaconPosition != newPosition && y.CanReach(newPosition)));
    }

    /// <summary>
    /// Find the distress beacon.
    /// </summary>
    /// <param name="sensors">The sensors in the room.</param>
    /// <param name="rowBoundary">The boundaries for row coordinates during the search.</param>
    /// <param name="columnBoundary">The boundaries for column coordinates during the search.</param>
    /// <returns>The position of the distress beacon.</returns>
    /// <exception cref="InvalidOperationException">Occurs when the distress beacon was not found.</exception>
    private static Position FindDistressBeacon(IReadOnlyList<Sensor> sensors, Vector rowBoundary, Vector columnBoundary)
    {
        var minX = Math.Max(sensors.Min(x => x.Position.X), rowBoundary.Start);
        var minY = Math.Max(sensors.Min(x => x.Position.Y), columnBoundary.Start);
        var maxX = Math.Min(sensors.Max(x => x.Position.X), rowBoundary.End);
        var maxY = Math.Min(sensors.Max(x => x.Position.Y), columnBoundary.End);

        return sensors
            .SelectMany(x => x.GetUnreachablePositions())
            .Where(x => x.X >= minX && x.X <= maxX && x.Y >= minY && x.Y <= maxY)
            .Distinct()
            .First(position => sensors.All(sensor => !sensor.CanReach(position)));
    }

    /// <summary>
    /// Gets the dimensions of the room.
    /// </summary>
    /// <param name="sensors">The sensors in the room.</param>
    /// <returns>A tuple with the width and height of the room.</returns>
    /// <exception cref="ArgumentNullException">Occurs when <paramref name="sensors"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Occurs when <paramref name="sensors"/> is empty.</exception>
    private static (Vector Width, Vector Height) GetRoomDimensions(IReadOnlyList<Sensor> sensors)
    {
        ArgumentNullException.ThrowIfNull(sensors, nameof(sensors));

        if (!sensors.Any())
            throw new ArgumentException("Sensor collection cannot be empty.", nameof(sensors));

        // Get the sensors that reach the furthest
        var minX = sensors.MinBy(x => x.Position.X - x.BeaconDistance)!;
        var minY = sensors.MinBy(x => x.Position.Y - x.BeaconDistance)!;
        var maxX = sensors.MaxBy(x => x.Position.X + x.BeaconDistance)!;
        var maxY = sensors.MaxBy(x => x.Position.Y + x.BeaconDistance)!;

        // Return the maximum reach of the sensor's range
        return (
            new(minX.Position.X - minX.BeaconDistance, maxX.Position.X + maxX.BeaconDistance),
            new(minY.Position.Y - minY.BeaconDistance, maxY.Position.Y + maxY.BeaconDistance)
        );
    }

    /// <summary>
    /// Source generated regex to parse sensor data from raw input data.
    /// </summary>
    /// <returns>A regex that parses sensor data.</returns>
    [GeneratedRegex(@"(-?\d+)", RegexOptions.Compiled)]
    private static partial Regex SensorRegex();
}