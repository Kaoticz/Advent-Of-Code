using _14.Enums;
using _14.Models;
using _14.Services;
using System.Collections.Immutable;

namespace _14;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");
    private static readonly Position _sandSpawnPoint = new(Content.Sand, 500, 0);

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);
        var scan = input
            .SelectMany(ParseScan)
            .Distinct()
            .ToImmutableArray();

        var result1 = RunSimulation(scan, _sandSpawnPoint);
        var result2 = RunSimulation(scan, _sandSpawnPoint, scan.Max(x => x.Y) + 2);

        Console.WriteLine($"First answer: {result1.Count(x => x.Content is Content.Sand)}");
        Console.WriteLine($"Second answer: {result2.Count(x => x.Content is Content.Sand)}");
    }

    /// <summary>
    /// Parses one line of raw input data.
    /// </summary>
    /// <param name="input">The raw input data.</param>
    /// <returns>The positions defined in the <paramref name="input"/>.</returns>
    private static IEnumerable<Position> ParseScan(string input)
    {
        var result = Enumerable.Empty<Position>();
        var coordinates = input
            .Split("->")
            .Select(x => decimal.Parse(x.Replace(',', '.')))
            .ToImmutableArray();

        for (var index = 1; index < coordinates.Length; index++)
            result = result.Concat(Cave.ParseCoordinates(coordinates[index - 1], coordinates[index]));

        return result;
    }

    /// <summary>
    /// Executes the sand simulation.
    /// </summary>
    /// <param name="positions">The positions in the room currently filled in.</param>
    /// <param name="sandSpawnPoint">The position where sand grains come from.</param>
    /// <param name="infiniteFloorY">The Y position of the infinite floor, <see langword="null"/> if there is none.</param>
    /// <returns>The content of the room after the simulation finishes running.</returns>
    private static IReadOnlySet<Position> RunSimulation(IEnumerable<Position> positions, Position sandSpawnPoint, int? infiniteFloorY = default)
    {
        var result = positions.ToHashSet();
        var currentSand = SpawnSand(result, sandSpawnPoint, infiniteFloorY);

        while (currentSand is not null && currentSand != sandSpawnPoint)
        {
            result.Add(currentSand);
            currentSand = SpawnSand(result, sandSpawnPoint, infiniteFloorY);
        }

        if (currentSand == sandSpawnPoint)
            result.Add(currentSand);

        return result;
    }

    /// <summary>
    /// Spawns a grain of sand.
    /// </summary>
    /// <param name="positions">The positions in the room currently filled in.</param>
    /// <param name="sandSpawnPoint">The position where sand grains come from.</param>
    /// <param name="infiniteFloorY">The Y position of the infinite floor, <see langword="null"/> if there is none.</param>
    /// <returns>The position of the new grain of sand, <see langword="null"/> if sand starts falling into the void.</returns>
    private static Position? SpawnSand(IReadOnlyCollection<Position> positions, Position sandSpawnPoint, int? infiniteFloorY)
    {
        var sandX = sandSpawnPoint.X;
        var sandY = sandSpawnPoint.Y;
        var movement = PredictSandMovement(positions, sandX, sandY);

        // If sand is falling in a bottomless pit or comes to rest, stop looping
        for (var counter = 0; counter < 200 && movement is not null; counter++)
        {
            // If sand hits something, reset counter to 0
            counter = (movement is Move.Bottom)
                ? counter
                : 0;

            // Move the sand
            sandY++;
            sandX += (int)movement;

            // If sand does not hit the infinite floor
            movement = (!infiniteFloorY.HasValue || sandY < infiniteFloorY - 1)
                ? PredictSandMovement(positions, sandX, sandY)  // Calculate its movement
                : null;                                         // Else, rest the sand
        }

        // If sand comes to rest
        return (movement is null)
            ? new(Content.Sand, sandX, sandY)   // Return its final position
            : null; // Else, return no position (simulation stop, no more sand can be spawned)
    }

    /// <summary>
    /// Determines the next sand movement according to the sand's current position.
    /// </summary>
    /// <param name="positions">The positions in the room currently filled in.</param>
    /// <param name="sandPositionX">The sand's X position.</param>
    /// <param name="sandPositionY">The sand's Y position.</param>
    /// <returns>The direction the sand is headed towards to, <see langword="null"/> if it comes to rest.</returns>
    private static Move? PredictSandMovement(IReadOnlyCollection<Position> positions, int sandPositionX, int sandPositionY)
    {
        return (!positions.Any(x => x.X == sandPositionX && x.Y == sandPositionY + 1))
            ? Move.Bottom
            : (!positions.Any(x => x.X == sandPositionX - 1 && x.Y == sandPositionY + 1))
                ? Move.BottomLeft
                : (!positions.Any(x => x.X == sandPositionX + 1 && x.Y == sandPositionY + 1))
                    ? Move.BottomRight
                    : null;
    }
}