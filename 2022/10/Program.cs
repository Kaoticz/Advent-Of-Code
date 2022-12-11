using _10.Enums;
using _10.Models;
using System.Collections.Immutable;
using System.Diagnostics;

namespace _10;

internal sealed class Program
{
    private readonly static string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);
        var cycles = ProcessOperations(input.Select(ParseOperation));
        var signalStrengths = CalculateSignalStrength(cycles, 20, 60, 100, 140, 180, 220);

        Console.WriteLine($"First answer: {signalStrengths.Sum()}");
        Console.WriteLine($"Second answer: {Environment.NewLine}{GenerateScreenImage(cycles, 40)}");
    }

    /// <summary>
    /// Generates a screen image based on the specified CPU cycles.
    /// </summary>
    /// <param name="cycles">The CPU cycles.</param>
    /// <param name="screenWidth">The width of the screen.</param>
    /// <returns>The screen image.</returns>
    private static string GenerateScreenImage(IReadOnlyList<Cycle> cycles, in int screenWidth)
    {
        var screen = Enumerable.Repeat('.', cycles.Count)
            .Chunk(screenWidth)
            .ToImmutableArray();

        // CRT draws pixels from 0 to 240, sequentially, in chunks of 'monitorWidth'
        // A sprite is always 3 pixels wide, horizontally
        // XValue sets the position of the sprite, from its center
        // If the sprite position and the CRT drawing position overlap, it produces a '#' pixel
        // Otherwise, it produces a '.' pixel.
        for (var index = 0; index < cycles.Count; index++)
        {
            var crt = index % screenWidth;
            var xValue = cycles[index].XValue;

            if (crt >= xValue - 1 && crt <= xValue + 1)
                screen[index / screenWidth][crt] = '#';
        }

        return string.Join(Environment.NewLine, screen.Select(x => string.Join(string.Empty, x)));
    }

    /// <summary>
    /// Calculates the signal strength for the specified CPU cycles.
    /// </summary>
    /// <param name="cycles">The CPU cycles.</param>
    /// <param name="cycleIds">The Ids of the CPU cycles.</param>
    /// <returns>The signal strengths of the specified cyles.</returns>
    private static IReadOnlyList<int> CalculateSignalStrength(IEnumerable<Cycle> cycles, params int[] cycleIds)
    {
        return cycles
            .Where(x => cycleIds.Contains(x.Id))
            .Select(x => x.Id * x.XValue)
            .ToImmutableArray();
    }

    /// <summary>
    /// Converts a set of operations into its corresponding CPU cycles.
    /// </summary>
    /// <param name="operations">The operations to be processed.</param>
    /// <returns>A list of CPU <see cref="Cycle"/>s.</returns>
    private static IReadOnlyList<Cycle> ProcessOperations(IEnumerable<Operation> operations)
    {
        var xValue = 1;
        var currentCycle = 1;
        var cycles = new List<Cycle>();

        foreach (var operation in operations)
        {
            var cyclesUsed = (int)operation.Type;

            while (cyclesUsed-- > 0)
                cycles.Add(new(currentCycle++, operation.Type, xValue));

            xValue += operation.ToAdd;
        }

        return cycles;
    }
    
    /// <summary>
    /// Parses raw input data of an operation.
    /// </summary>
    /// <param name="operation">The operation to be parsed.</param>
    /// <returns>A parsed <see cref="Operation"/>.</returns>
    /// <exception cref="UnreachableException">Occurs when the operation is not recognized.</exception>
    private static Operation ParseOperation(string operation)
    {
        var operationType = operation.AsSpan()[..4] switch
        {
            "noop" => OperationType.Noop,
            "addx" => OperationType.Addx,
            _ => throw new UnreachableException($"Operation of type {operation.AsSpan()[..4]} is unknown.")
        };

        var toAdd = (operationType is OperationType.Addx)
            ? int.Parse(operation.AsSpan()[4..])
            : 0;

        return new(operationType, toAdd);
    }
}