namespace _07.Models;

/// <summary>
/// Represents a system file.
/// </summary>
/// <param name="Name">The name of the file.</param>
/// <param name="Size">The size of the file, in bytes.</param>
internal sealed record SystemFile(string Name, int Size);