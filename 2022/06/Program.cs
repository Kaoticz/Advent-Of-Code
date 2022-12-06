namespace _06;

internal sealed class Program
{
    private readonly static string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllTextAsync(_inputLocation);

        Console.WriteLine($"First answer: {MarkerEndsAt(input, 4)}");
        Console.WriteLine($"Second answer: {MarkerEndsAt(input, 14)}");
    }

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