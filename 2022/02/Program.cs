using _02.Enums;
using System.Diagnostics;

namespace _02;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);

        var result1 = input
            .Select(x => (ElfBet: ParseBet(x[0]), PlayerBet: ParseBet(x[2])))
            .Sum(x => (int)x.PlayerBet + (int)ResolveOutcome(x.PlayerBet, x.ElfBet));

        var result2 = input
            .Select(x => (ElfBet: ParseBet(x[0]), Outcome: ParseOutcome(x[2])))
            .Sum(x => (int)ResolvePlayerBet(x.ElfBet, x.Outcome) + (int)x.Outcome);

        Console.WriteLine($"First answer: {result1}");
        Console.WriteLine($"Second answer: {result2}");
    }

    /// <summary>
    /// Parses the outcome of a game for the specified raw <paramref name="outcome"/> input.
    /// </summary>
    /// <param name="outcome">The raw outcome of the game.</param>
    /// <returns>The outcome of the game.</returns>
    /// <exception cref="UnreachableException">Occurs when raw <paramref name="outcome"/> is not recognized.</exception>
    public static Outcome ParseOutcome(char outcome)
    {
        return outcome switch
        {
            'X' => Outcome.Defeat,
            'Y' => Outcome.Draw,
            'Z' => Outcome.Victory,
            _ => throw new UnreachableException($"Match outcome of value {outcome} is unknown.")
        };
    }

    /// <summary>
    /// Parses a raw player bet.
    /// </summary>
    /// <param name="bet">The raw player bet.</param>
    /// <returns>The player's bet.</returns>
    /// <exception cref="UnreachableException">Occurs when raw <paramref name="bet"/> is not recognized.</exception>
    public static BetType ParseBet(char bet)
    {
        return bet switch
        {
            'A' or 'X' => BetType.Rock,
            'B' or 'Y' => BetType.Paper,
            'C' or 'Z' => BetType.Scissors,
            _ => throw new UnreachableException($"Player bet of value {bet} is unknown.")
        };
    }

    /// <summary>
    /// Gets the outcome of a game for the specified bets.
    /// </summary>
    /// <param name="playerBet">The player's bet.</param>
    /// <param name="elfBet">The opponent's bet.</param>
    /// <returns>The outcome of the game.</returns>
    /// <exception cref="UnreachableException">Occurs when raw <paramref name="playerBet"/> is not recognized.</exception>
    public static Outcome ResolveOutcome(BetType playerBet, BetType elfBet)
    {
        return playerBet switch
        {
            BetType.Rock when elfBet is BetType.Rock => Outcome.Draw,
            BetType.Rock when elfBet is BetType.Paper => Outcome.Defeat,
            BetType.Rock when elfBet is BetType.Scissors => Outcome.Victory,
            BetType.Paper when elfBet is BetType.Rock => Outcome.Victory,
            BetType.Paper when elfBet is BetType.Paper => Outcome.Draw,
            BetType.Paper when elfBet is BetType.Scissors => Outcome.Defeat,
            BetType.Scissors when elfBet is BetType.Rock => Outcome.Defeat,
            BetType.Scissors when elfBet is BetType.Paper => Outcome.Victory,
            BetType.Scissors when elfBet is BetType.Scissors => Outcome.Draw,
            _ => throw new UnreachableException($"Player bet of value {playerBet} is unknown.")
        };
    }

    /// <summary>
    /// Resolves the appropriate player bet given the specified opponent's bet and the desired game outcome.
    /// </summary>
    /// <param name="elfBet">The opponent's bet.</param>
    /// <param name="gameOutcome">The desired game outcome.</param>
    /// <returns>The bet that should be played.</returns>
    /// <exception cref="UnreachableException">Occurs when raw <paramref name="gameOutcome"/> is not recognized.</exception>
    public static BetType ResolvePlayerBet(BetType elfBet, Outcome gameOutcome)
    {
        return gameOutcome switch
        {
            Outcome.Defeat when elfBet is BetType.Rock => BetType.Scissors,
            Outcome.Defeat when elfBet is BetType.Paper => BetType.Rock,
            Outcome.Defeat when elfBet is BetType.Scissors => BetType.Paper,
            Outcome.Draw when elfBet is BetType.Rock => BetType.Rock,
            Outcome.Draw when elfBet is BetType.Paper => BetType.Paper,
            Outcome.Draw when elfBet is BetType.Scissors => BetType.Scissors,
            Outcome.Victory when elfBet is BetType.Rock => BetType.Paper,
            Outcome.Victory when elfBet is BetType.Paper => BetType.Scissors,
            Outcome.Victory when elfBet is BetType.Scissors => BetType.Rock,
            _ => throw new UnreachableException($"Game outcome of value {gameOutcome} is unknown.")
        };
    }
}