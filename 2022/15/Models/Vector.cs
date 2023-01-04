namespace _15.Models;

/// <summary>
/// Represents an arbitrary unidimensional vector.
/// </summary>
internal sealed record Vector
{
    /// <summary>
    /// Defines the starting position of this vector.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Defines the ending position of this vector.
    /// </summary>
    public int End { get; }

    /// <summary>
    /// Represents the length of this vector.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Initializes a unidimensional vector.
    /// </summary>
    /// <param name="start">The position where the vector starts.</param>
    /// <param name="end">The position where the vector ends.</param>
    public Vector(int start, int end)
    {
        Start = Math.Min(start, end);
        End = Math.Max(start, end);
        Length = End - Start + 1;
    }
}