using System.Collections.Immutable;

namespace _01;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllTextAsync(_inputLocation);
        var result = input.Split(Environment.NewLine + Environment.NewLine)
            .Select(caloriesGroup =>
                caloriesGroup.Split(Environment.NewLine)
                    .Sum(int.Parse)
            )
            .OrderDescending()
            .Take(3)
            .ToImmutableArray();

        Console.WriteLine($"First answer: {result[0]}");
        Console.WriteLine($"Second answer: {result.Sum()}");
    }
}