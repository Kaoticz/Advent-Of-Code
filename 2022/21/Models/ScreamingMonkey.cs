using _21.Models.Abstractions;

namespace _21.Models;

/// <summary>
/// Represents a monkey that only screams a set number.
/// </summary>
public sealed record ScreamingMonkey : Monkey
{
    /// <summary>
    /// The number screamed by this monkey.
    /// </summary>
    public long Number { get; }

    /// <summary>
    /// Initializes a screaming monkey.
    /// </summary>
    /// <param name="name">The name of the monkey.</param>
    /// <param name="number">The number screamed by this monkey.</param>
    public ScreamingMonkey(string name, long number) : base(name)
        => Number = number;
    
    /// <inheritdoc />
    public override long GetValue()
        => Number;
}