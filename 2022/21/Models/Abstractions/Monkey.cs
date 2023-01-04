namespace _21.Models.Abstractions;

/// <summary>
/// Represents a monkey.
/// </summary>
public abstract record Monkey
{
    /// <summary>
    /// The name of the monkey.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a monkey.
    /// </summary>
    /// <param name="name">The name of the monkey.</param>
    public Monkey(string name)
        => Name = name;

    /// <summary>
    /// Gets the number screamed by this monkey.
    /// </summary>
    /// <returns>The number of this monkey.</returns>
    public abstract long GetValue();
}