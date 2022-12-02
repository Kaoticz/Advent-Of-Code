namespace _02;

internal sealed class Program
{
    private readonly static string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);

        var result1 = input
            .Select(x => (ElfBet: GameResolver.ParseBet(x[0]), PlayerBet: GameResolver.ParseBet(x[2])))
            .Sum(x => (int)x.PlayerBet + (int)GameResolver.ResolveOutcome(x.PlayerBet, x.ElfBet));

        var result2 = input
            .Select(x => (ElfBet: GameResolver.ParseBet(x[0]), Outcome: GameResolver.ParseOutcome(x[2])))
            .Sum(x => (int)GameResolver.ResolvePlayerBet(x.ElfBet, x.Outcome) + (int)x.Outcome);

        Console.WriteLine($"First answer: {result1}");
        Console.WriteLine($"Second answer: {result2}");
    }
}