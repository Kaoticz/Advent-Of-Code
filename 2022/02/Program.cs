namespace _02;

internal sealed class Program
{
    private readonly static string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);

        var result1 = input
            .Select(x => (int)GameResolver.ParseBet(x[2]) + (int)GameResolver.ResolveOutcome(x[2], x[0]))
            .Sum();

        var result2 = input
            .Select(x => (int)GameResolver.ResolvePlayerBet(x[0], x[2]) + (int)GameResolver.ParseOutcome(x[2]))
            .Sum();

        Console.WriteLine($"First answer: {result1}");
        Console.WriteLine($"Second answer: {result2}");
    }
}