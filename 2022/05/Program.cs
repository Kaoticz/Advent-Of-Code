using _05.Models;
using System.Collections.Immutable;

namespace _05;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);
        var stacks1 = ParseCrateStacks(input.AsSpan()[..8], ParseStackAmount(input[8]));
        var stacks2 = ParseCrateStacks(input.AsSpan()[..8], ParseStackAmount(input[8]));

        foreach (var instruction in input.Skip(10).Select(ParseInstruction))
        {
            MoveCrates(stacks1, instruction);
            MoveCratesInBulk(stacks2, instruction);
        }

        Console.WriteLine($"First answer: {string.Join(string.Empty, stacks1.Select(x => x.Peek()))}");
        Console.WriteLine($"Second answer: {string.Join(string.Empty, stacks2.Select(x => x.Peek()))}");
    }

    /// <summary>
    /// Parses the content of the crate stacks from the raw data.
    /// </summary>
    /// <param name="crateLines">The raw input.</param>
    /// <param name="stackAmount">The amount of stacks to be created.</param>
    /// <returns>A collection of stacks with the input data.</returns>
    private static ImmutableArray<Stack<char>> ParseCrateStacks(ReadOnlySpan<string> crateLines, int stackAmount)
    {
        var stacks = Enumerable.Range(0, stackAmount)
            .Select(_ => new Stack<char>())
            .ToImmutableArray();

        for (var stackIndex = 0; stackIndex < stacks.Length; stackIndex++)
        {
            for (var crateLineIndex = crateLines.Length - 1; crateLineIndex >= 0; crateLineIndex--)
            {
                var crate = crateLines[crateLineIndex][(stackIndex * 4) + 1];

                if (char.IsWhiteSpace(crate))
                    break;

                stacks[stackIndex].Push(crate);
            }
        }

        return stacks;
    }

    /// <summary>
    /// Parses the amount of stacks defined in the raw data.
    /// </summary>
    /// <param name="stackIdentifiers">The line of stack identifiers.</param>
    /// <returns>The amount of stacks required by the instructions.</returns>
    private static int ParseStackAmount(string stackIdentifiers)
    {
        return stackIdentifiers.Split(' ')
            .Count(x => int.TryParse(x, out _));
    }

    /// <summary>
    /// Parses one line of instruction raw data.
    /// </summary>
    /// <param name="instructionLine">The line to be parsed.</param>
    /// <returns>An <see cref="Instruction"/> object.</returns>
    private static Instruction ParseInstruction(string instructionLine)
    {
        var result = instructionLine.Split(' ')
            .Where(x => int.TryParse(x, out _))
            .Select(int.Parse)
            .ToImmutableArray();

        return new Instruction(result[0], result[1] - 1, result[2] - 1);
    }

    /// <summary>
    /// Moves crates one by one from one stack to another.
    /// </summary>
    /// <param name="stacks">The collection of stacks.</param>
    /// <param name="instruction">The operation instruction.</param>
    private static void MoveCrates(IReadOnlyList<Stack<char>> stacks, Instruction instruction)
    {
        for (var operation = 0; operation < instruction.Amount; operation++)
            stacks[instruction.DestinationStack].Push(stacks[instruction.SourceStack].Pop());
    }

    /// <summary>
    /// Moves crates in bulk from one stack to another.
    /// </summary>
    /// <param name="stacks">The collection of stacks.</param>
    /// <param name="instruction">The operation instruction.</param>
    private static void MoveCratesInBulk(IReadOnlyList<Stack<char>> stacks, Instruction instruction)
    {
        var tempStack = new Stack<char>();

        // Pull creates from origin stack
        for (var counter = 0; counter < instruction.Amount; counter++)
            tempStack.Push(stacks[instruction.SourceStack].Pop());

        // Place crates on destination stack
        for (var counter = 0; counter < instruction.Amount; counter++)
            stacks[instruction.DestinationStack].Push(tempStack.Pop());
    }
}