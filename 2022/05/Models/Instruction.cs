namespace _05.Models;

/// <summary>
/// Represents an instruction on how to move crates across crate stacks.
/// </summary>
internal readonly struct Instruction
{
    /// <summary>
    /// The amount of crates to be moved.
    /// </summary>
    public int Amount { get; }

    /// <summary>
    /// The crate stack to get the crates from.
    /// </summary>
    public int SourceStack { get; }

    /// <summary>
    /// The crate stack to send the crates to.
    /// </summary>
    public int DestinationStack { get; }

    /// <summary>
    /// Creates an instruction for moving crates from crate stacks.
    /// </summary>
    /// <param name="amount">The amount of crates to be moved.</param>
    /// <param name="fromStack">The crate stack to get the crates from.</param>
    /// <param name="toStack">The crate stack to send the crates to.</param>
    public Instruction(int amount, int fromStack, int toStack)
    {
        Amount = amount;
        SourceStack = fromStack;
        DestinationStack = toStack;
    }
}
