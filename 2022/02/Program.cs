namespace _02;

internal sealed class Program
{
    private readonly static string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllTextAsync(_inputLocation);
        Console.WriteLine(input);
    }
}