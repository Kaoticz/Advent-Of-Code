using _07.Models;
using System.Collections.Immutable;

namespace _07;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");
    private const int _totalSpace = 70000000;
    private const int _spaceRequired = 30000000;

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);
        var fileSystem = ExecuteCommands(ParseCommands(input));
        var minimumSpaceToFree = _spaceRequired - (_totalSpace - CalculateRootSize(fileSystem));
        var result1 = fileSystem
            .Select(x => CalculateDirectorySize(fileSystem, x.Key))
            .Where(x => x <= 100000)
            .Sum();

        var result2 = fileSystem
            .Select(x => CalculateDirectorySize(fileSystem, x.Key))
            .Where(x => x >= minimumSpaceToFree)
            .Min();

        Console.WriteLine($"First answer: {result1}");
        Console.WriteLine($"Second answer: {result2}");
    }

    /// <summary>
    /// Parses shell commands from raw input data.
    /// </summary>
    /// <param name="inputLines">The lines of the raw input data.</param>
    /// <returns>The commands executed in the shell.</returns>
    private static IReadOnlyList<Command> ParseCommands(IReadOnlyList<string> inputLines)
    {
        var commands = new List<Command>();

        for (var index = 0; index < inputLines.Count; index++)
        {
            if (!inputLines[index].StartsWith("$"))
                continue;

            commands.Add(
                new(
                    inputLines[index][2..],
                    inputLines
                        .Skip(index + 1)
                        .TakeWhile(x => !x.StartsWith('$'))
                        .ToImmutableArray()
                )
            );
        }

        return commands;
    }

    /// <summary>
    /// Executes multiple shell commands.
    /// </summary>
    /// <param name="commands">The commands to be executed.</param>
    /// <returns>The system's file structure.</returns>
    /// <exception cref="InvalidOperationException">Occurs when an invalid command is executed.</exception>
    private static IReadOnlyDictionary<string, IReadOnlySet<SystemFile>> ExecuteCommands(IReadOnlyList<Command> commands)
    {
        var currentDir = string.Empty;
        var fileSystem = new Dictionary<string, IReadOnlySet<SystemFile>>();

        foreach (var command in commands)
        {
            if (command.InputCommand.StartsWith("cd"))
                currentDir = ExecuteCdCommand(command, currentDir);
            else if (command.InputCommand.StartsWith("ls"))
                fileSystem.Add(currentDir, ExecuteLsCommand(command));
            else
                throw new InvalidOperationException($"Command '{command.InputCommand}' is not recognized.");
        }

        return fileSystem;
    }

    /// <summary>
    /// Executes a 'cd' shell command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    /// <param name="currentDir">The current directory.</param>
    /// <returns>The directory the shell has moved to.</returns>
    /// <exception cref="ArgumentException">Occurs when the method is executed with any command other than 'ls'.</exception>
    private static string ExecuteCdCommand(Command command, string currentDir)
    {
        if (!command.InputCommand.StartsWith("cd"))
            throw new ArgumentException($"Expected command 'cd', got '{command.InputCommand}' instead.");

        if (command.InputCommand.Contains(".."))
        {
            var lastPathSeparator = (currentDir.LastIndexOf(Path.DirectorySeparatorChar) is 0)
                ? 1
                : currentDir.LastIndexOf(Path.DirectorySeparatorChar);

            currentDir = currentDir[..lastPathSeparator];
        }
        else
        {
            currentDir = (command.InputCommand[3..] is "/")
                ? Path.DirectorySeparatorChar.ToString()
                : Path.Combine(currentDir, command.InputCommand[3..]);
        }

        return currentDir;
    }

    /// <summary>
    /// Executes a 'ls' shell command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    /// <returns>The files found in the current folder.</returns>
    /// <exception cref="ArgumentException">Occurs when the method is executed with any command other than 'ls'.</exception>
    private static IReadOnlySet<SystemFile> ExecuteLsCommand(Command command)
    {
        if (!command.InputCommand.StartsWith("ls"))
            throw new ArgumentException($"Expected command 'ls', got '{command.InputCommand}' instead.");

        var result = new HashSet<SystemFile>();

        foreach (var output in command.Output)
        {
            var splitOutput = output.Split(' ');

            if (int.TryParse(splitOutput[0], out var size))
                result.Add(new(splitOutput[1], size));
        }

        return result;
    }

    /// <summary>
    /// Calculates the size of the root directory.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <returns>The size of the root directory, in bytes.</returns>
    private static int CalculateRootSize(IReadOnlyDictionary<string, IReadOnlySet<SystemFile>> fileSystem)
        => CalculateDirectorySize(fileSystem, Path.DirectorySeparatorChar.ToString());

    /// <summary>
    /// Calculates the size of the given directory.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The absolute path to the directory.</param>
    /// <returns>The size of the directory, in bytes.</returns>
    private static int CalculateDirectorySize(IReadOnlyDictionary<string, IReadOnlySet<SystemFile>> fileSystem, string path)
    {
        return fileSystem
            .Where(x => x.Key.StartsWith(path))
            .SelectMany(x => x.Value)
            .Sum(x => x.Size);
    }
}