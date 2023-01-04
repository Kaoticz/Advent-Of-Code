using _21.Models;
using _21.Models.Abstractions;
using Kotz.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace _21;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var monkeys = ParseMonkeys(await File.ReadAllLinesAsync(_inputLocation));

        Console.WriteLine($"First answer: {monkeys["root"].GetValue()}");
        Console.WriteLine($"Second answer: {CalculateComplement(monkeys["root"], monkeys["humn"])}");
    }

    /// <summary>
    /// Parses <see cref="Monkey"/>s from raw input data.
    /// </summary>
    /// <param name="input">Lines of raw input data.</param>
    /// <returns>The parsed <see cref="Monkey"/>s.</returns>
    private static IReadOnlyDictionary<string, Monkey> ParseMonkeys(IReadOnlyList<string> input)
    {
        var result = new Dictionary<string, Monkey>();
        var monkeyQueue = new Queue<string>(input);

        while (monkeyQueue.TryDequeue(out var rawMonkey))
        {
            if (TryParseScreamingMonkey(rawMonkey, out var screamingMonkey))
                result.Add(screamingMonkey.Name, screamingMonkey);
            else if (TryParseMathMonkey(rawMonkey, result, out var mathMonkey))
                result.Add(mathMonkey.Name, mathMonkey);
            else
                monkeyQueue.Enqueue(rawMonkey);
        }

        return result;
    }

    /// <summary>
    /// Parses a <see cref="MathMonkey"/> from raw input data.
    /// </summary>
    /// <param name="input">The raw input data.</param>
    /// <param name="parsedMonkeys">The monkeys currently parsed so far.</param>
    /// <param name="monkey">The parsed monkey, or <see langword="null"/> if parsing did not succeed.</param>
    /// <returns><see langword="true"/> if parsing was successful, <see langword="false"/> otherwise.</returns>
    private static bool TryParseMathMonkey(string input, IReadOnlyDictionary<string, Monkey> parsedMonkeys, [MaybeNullWhen(false)] out MathMonkey monkey)
    {
        var splitInput = input.Split(':', ' ');  // ["monkey", "", "monkey1", "operator", "monkey2"]

        if (parsedMonkeys.TryGetValue(splitInput[2], out var leftMonkey) && parsedMonkeys.TryGetValue(splitInput[4], out var rightMonkey))
        {
            monkey = new(splitInput[0], splitInput[3][0], leftMonkey, rightMonkey);
            return true;
        }

        monkey = default;
        return false;
    }

    /// <summary>
    /// Parses a <see cref="ScreamingMonkey"/> from raw input data.
    /// </summary>
    /// <param name="input">The raw input data.</param>
    /// <param name="monkey">The parsed monkey, or <see langword="null"/> if parsing did not succeed.</param>
    /// <returns><see langword="true"/> if parsing was successful, <see langword="false"/> otherwise.</returns>
    private static bool TryParseScreamingMonkey(string input, [MaybeNullWhen(false)] out ScreamingMonkey monkey)
    {
        var splitInput = input.Split(':');  // ["monkey", "number"]

        if (long.TryParse(splitInput[1], out var number))
        {
            monkey = new(splitInput[0], number);
            return true;
        }

        monkey = default;
        return false;
    }

    /// <summary>
    /// Gets the dependency tree between two monkeys.
    /// </summary>
    /// <param name="root">The monkey at the root of the tree.</param>
    /// <param name="dependency">The monkey at the end of the tree.</param>
    /// <returns>The dependency tree between <paramref name="root"/> and <paramref name="dependency"/>.</returns>
    /// <exception cref="InvalidOperationException">Occurs when <paramref name="root"/> does not depend on <paramref name="dependency"/>.</exception>
    private static IEnumerable<Monkey> GetDependencyTree(Monkey root, Monkey dependency)
    {
        var result = Enumerable.Empty<Monkey>();

        if (root == dependency)
            return result.Append(root);
        else if (root is MathMonkey mathStart)
        {
            if (TreeContainsDependency(mathStart.LeftMonkey, dependency))
            {
                return result
                    .Append(mathStart)
                    .Concat(GetDependencyTree(mathStart.LeftMonkey, dependency));
            }
            else if (TreeContainsDependency(mathStart.RightMonkey, dependency))
            {
                return result
                    .Append(mathStart)
                    .Concat(GetDependencyTree(mathStart.RightMonkey, dependency));
            }
        }

        throw new InvalidOperationException($"'{dependency.Name}' is not a dependency of '{root.Name}'.");
    }

    /// <summary>
    /// Checks if the <paramref name="dependency"/> monkey is contained within the
    /// dependency tree of the <paramref name="root"/> monkey.
    /// </summary>
    /// <param name="root">The monkey at the root of the tree.</param>
    /// <param name="dependency">The monkey at the end of the tree.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="dependency"/> is present in
    /// <paramref name="root"/>'s dependency tree, <see langword="false"/> otherwise.
    /// </returns>
    private static bool TreeContainsDependency(Monkey root, Monkey dependency)
    {
        return (root is MathMonkey calcStart)
            ? TreeContainsDependency(calcStart.LeftMonkey, dependency) || TreeContainsDependency(calcStart.RightMonkey, dependency)
            : root == dependency;
    }

    /// <summary>
    /// Calculates the complement number that should be yelled by <paramref name="end"/>
    /// so it equalizes the number yelled by <paramref name="start"/>.
    /// </summary>
    /// <param name="start">The monkey at the root of the desired tree.</param>
    /// <param name="end">The monkey at the end of the desired tree.</param>
    /// <returns>The complement number for the specified monkeys.</returns>
    private static long CalculateComplement(Monkey start, Monkey end)
    {
        if (start is not MathMonkey calcMonkey)
            return -start.GetValue();

        var dependencyTree = GetDependencyTree(calcMonkey, end)
            .ToArray();

        var finalComparison = (dependencyTree.Contains(calcMonkey.LeftMonkey))
            ? calcMonkey.RightMonkey.GetValue()
            : calcMonkey.LeftMonkey.GetValue();

        for (var index = 2; index < dependencyTree.Length; index++)
        {
            var monkey = (MathMonkey)dependencyTree[index - 1];
            var dependencyMonkey = dependencyTree[index];

            finalComparison = (dependencyMonkey == monkey.LeftMonkey)
                ? MathMonkey.ExecuteOperation(finalComparison, monkey.RightMonkey.GetValue(), monkey.ReverseOperator)
                : (monkey.Operator is '-')
                    ? MathMonkey.ExecuteOperation(monkey.LeftMonkey.GetValue(), finalComparison, monkey.Operator)
                    : MathMonkey.ExecuteOperation(finalComparison, monkey.LeftMonkey.GetValue(), monkey.ReverseOperator);
        }

        return finalComparison;
    }
}