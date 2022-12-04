using System.Collections.Immutable;

namespace _04;

internal sealed class Program
{
    private readonly static string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var result = (await File.ReadAllLinesAsync(_inputLocation))
            .Select(x => x.Split(',').Select(ParseSection).ToImmutableArray())
            .ToImmutableArray();

        Console.WriteLine($"First answer: {result.Count(x => IsRangeContained(x[0], x[1]))}");
        Console.WriteLine($"Second answer: {result.Count(x => IsRangeOverlapping(x[0], x[1]))}");
    }

    /// <summary>
    /// Parses a section into a <see cref="Range"/>.
    /// </summary>
    /// <param name="section">A string with two numbers separated by '-'.</param>
    /// <returns>The <see cref="Range"/> of a section.</returns>
    private static Range ParseSection(string section)
    {
        var sectors = section.Split('-');
        return new Range(int.Parse(sectors[0]), int.Parse(sectors[1]));
    }

    /// <summary>
    /// Checks if one <see cref="Range"/> is contained within another.
    /// </summary>
    /// <param name="first">The first range.</param>
    /// <param name="second">The second range.</param>
    /// <returns><see langword="true"/> if one of the ranges contains another, <see langword="false"/> otherwise.</returns>
    private static bool IsRangeContained(Range first, Range second)
    {
        return (first.Start.Value >= second.Start.Value && first.End.Value <= second.End.Value)
            || (second.Start.Value >= first.Start.Value && second.End.Value <= first.End.Value);
    }

    /// <summary>
    /// Checks if one <see cref="Range"/> overlaps with another.
    /// </summary>
    /// <param name="first">The first range.</param>
    /// <param name="second">The second range.</param>
    /// <returns><see langword="true"/> if one of the ranges overlaps the other, <see langword="false"/> otherwise.</returns>
    private static bool IsRangeOverlapping(Range first, Range second)
    {
        return (first.End.Value >= second.Start.Value && first.Start.Value <= second.End.Value)
            || (second.End.Value >= first.Start.Value && second.Start.Value <= first.End.Value);
    }
}