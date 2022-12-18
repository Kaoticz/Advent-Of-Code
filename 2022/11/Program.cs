using Kotz.Extensions;
using System.Collections.Immutable;
using System.Diagnostics;

namespace _11;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);
        var monkeys1 = RunSimulation(input, 20, 3);
        var monkeys2 = RunSimulation(input, 10000, 1);

        Console.WriteLine($"First answer: {CalculateMonkeyBusiness(monkeys1)}");
        Console.WriteLine($"Second answer: {CalculateMonkeyBusiness(monkeys2)}");
    }

    /// <summary>
    /// Executes the 'Keep Away' simulation.
    /// </summary>
    /// <param name="input">The raw input data.</param>
    /// <param name="rounds">How many rounds should be executed.</param>
    /// <param name="worryReduceFactor">A factor that reduces the worry levels when the monkey gets bored with the inspected item.</param>
    /// <returns>The monkeys and their respective state when the simulation is over.</returns>
    private static IReadOnlyList<Monkey> RunSimulation(IReadOnlyList<string> input, int rounds, long worryReduceFactor)
    {
        var monkeys = input
            .Split(string.Empty)
            .Select(x => ParseMonkey(x.ToArray()))
            .ToImmutableArray();

        var divisors = monkeys
            .Select(x => x.Divisor)
            .Distinct()
            .Aggregate((x, y) => x * y);

        for (var amount = 0; amount < rounds; amount++)
            ExecuteRound(monkeys, divisors, worryReduceFactor);

        return monkeys;
    }

    /// <summary>
    /// Executes one round of 'Keep Away'.
    /// </summary>
    /// <param name="monkeys">The monkeys playing 'Keep Away'.</param>
    /// <param name="divisors">The divisors for the worry level.</param>
    /// <param name="worryReduceFactor">A factor that reduces the worry levels when the monkey gets bored with the inspected item.</param>
    private static void ExecuteRound(IReadOnlyList<Monkey> monkeys, long divisors, long worryReduceFactor)
    {
        foreach (var monkey in monkeys)
        {
            var amount = monkey.ItemsInPossession;

            while (amount-- > 0)
                monkey.ThrowItem(monkeys, divisors, worryReduceFactor);
        }
    }

    /// <summary>
    /// Calculates the monkey business of the specified group of monkeys.
    /// </summary>
    /// <param name="monkeys">The group of monkeys.</param>
    /// <returns>The monkey business for the group.</returns>
    private static long CalculateMonkeyBusiness(IEnumerable<Monkey> monkeys)
    {
        var topThrows = monkeys.Max(x => x.ThrowAmount);
        var secondBestThrows = monkeys
            .Where(x => x.ThrowAmount != topThrows)
            .Max(x => x.ThrowAmount);

        return topThrows * secondBestThrows;
    }

    /// <summary>
    /// Parses raw monkey data.
    /// </summary>
    /// <param name="monkeyRawData">The raw monkey data.</param>
    /// <returns>A parsed <see cref="Monkey"/>.</returns>
    /// <exception cref="UnreachableException">Occurs when the inspection operation is not recognized.</exception>
    private static Monkey ParseMonkey(IReadOnlyList<string> monkeyRawData)
    {
        // Parse ID
        var id = monkeyRawData[0][7] - '0';

        // Parse starting items
        var rawStartingItems = monkeyRawData[1].Split(':', ',')
            .Skip(1)
            .Select(long.Parse);

        // Parse operation
        var @operator = monkeyRawData[2].AsSpan()[23];
        var isOperandNumber = int.TryParse(monkeyRawData[2].AsSpan()[24..], out var operand);

        // Parse monkey choice condition
        var divisibleBy = int.Parse(monkeyRawData[3].AsSpan()[20..]);

        // Parse monkey choice results
        var trueThrow = monkeyRawData[4].AsSpan()[29] - '0';
        var falseThrow = monkeyRawData[5].AsSpan()[30] - '0';

        return new Monkey(
            id,
            divisibleBy,
            rawStartingItems,
            (@operator, isOperandNumber) switch
            {
                ('+', true) => (x => x + operand),
                ('+', false) => (x => x + x),
                ('*', true) => (x => x * operand),
                ('*', false) => (x => x * x),
                _ => throw new UnreachableException($"Operation of type '{@operator}' is unknown.")
            },
            x => x % divisibleBy is 0 ? trueThrow : falseThrow
        );
    }
}