namespace _06;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllTextAsync(_inputLocation);

        Console.WriteLine($"First answer: {MarkerEndsAt(input, 4)}");
        Console.WriteLine($"Second answer: {MarkerEndsAt(input, 14)}");
    }

    /// <summary>
    /// Finds a marker in <paramref name="text"/> with the specified <paramref name="markerLength"/>.
    /// </summary>
    /// <param name="text">The text to be analyzed.</param>
    /// <param name="markerLength">The length of the marker.</param>
    /// <returns>The index in <paramref name="text"/> where the marker ends, -1 if no marker is found.</returns>
    private static int MarkerEndsAt(string text, int markerLength)
    {
        for (var index = 0; index < text.Length; index++)
        {
            if (text.Substring(index, markerLength).Distinct().Count() == markerLength)
                return index + markerLength;
        }

        return -1;
    }
}