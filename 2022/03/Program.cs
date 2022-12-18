namespace _03;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);
        var result1 = input.Sum(x => ParsePriority(FindItemType(x)));
        var result2 = input
            .Chunk(3)
            .Sum(x => ParsePriority(FindRucksackBadge(x)));

        Console.WriteLine($"First answer: {result1}");
        Console.WriteLine($"Second answer: {result2}");
    }

    /// <summary>
    /// Finds the item type of a given <paramref name="rucksack"/>.
    /// </summary>
    /// <param name="rucksack">The rucksack to search in.</param>
    /// <returns>The item priority char of the <paramref name="rucksack"/>.</returns>
    private static char FindItemType(string rucksack)
    {
        return rucksack.First(x =>
        {
            var firstCompartment = rucksack[..(rucksack.Length / 2)];
            var secondCompartment = rucksack[(rucksack.Length / 2)..];

            return firstCompartment.Contains(x) && secondCompartment.Contains(x);
        });
    }

    /// <summary>
    /// Finds the priority char of a group of rucksacks.
    /// </summary>
    /// <param name="rucksacks">The rucksacks to search in.</param>
    /// <returns>The badge priority char of the rucksacks.</returns>
    private static char FindRucksackBadge(IReadOnlyList<string> rucksacks)
    {
        return rucksacks
            .SelectMany(x => x)
            .First(x => rucksacks.All(y => y.Contains(x)));
    }

    /// <summary>
    /// Calculates the priority value of a given <paramref name="priorityChar"/>.
    /// </summary>
    /// <param name="priorityChar">The priority char.</param>
    /// <returns>The priority value of <paramref name="priorityChar"/>.</returns>
    private static int ParsePriority(char priorityChar)
    {
        return (char.IsLower(priorityChar))
            ? priorityChar - 'a' + 1
            : priorityChar - 'A' + 27;
    }
}