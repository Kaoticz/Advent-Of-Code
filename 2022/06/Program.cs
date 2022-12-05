namespace _06;

internal sealed class Program
{
    private readonly static string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);
        Console.WriteLine(input);
    }
}
