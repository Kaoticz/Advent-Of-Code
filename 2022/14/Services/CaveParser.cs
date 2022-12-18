using _14.Enums;
using _14.Models;
using System.Diagnostics;

namespace _14.Services;

/// <summary>
/// Contains methods for reading and writing a cave room.
/// </summary>
internal static class Cave
{
    /// <summary>
    /// Parses one step of a cave scan.
    /// </summary>
    /// <param name="first">Left value.</param>
    /// <param name="second">Right value.</param>
    /// <returns>The positions defined in the scan.</returns>
    public static IEnumerable<Position> ParseCoordinates(decimal first, decimal second)
    {
        return ParseHorizontalStructure(first, second)
            .Concat(ParseVerticalStructure(first, second));
    }

    /// <summary>
    /// Generates a room visualization for the specified positions.
    /// </summary>
    /// <param name="positions">The positions to be printed.</param>
    /// <returns>The cave's room visualization.</returns>
    public static string GenerateRoomImage(IReadOnlyList<Position> positions)
    {
        var gridWidth = positions.Max(x => x.X) - positions.Min(x => x.X) + 1;
        var gridHeight = positions.Max(x => x.Y) - positions.Min(x => x.Y) + 1;

        var grid = Enumerable.Range(0, gridWidth * gridHeight)
            .Chunk(gridWidth)
            .Select((row, rowIndex) => row.Select((_, columnIndex) => new Position(Content.Air, columnIndex, rowIndex)).ToArray())
            .ToArray();

        foreach (var position in positions.Select(x => x with { X = x.X - positions.Min(y => y.X), Y = x.Y - positions.Min(y => y.Y) }))
            grid[position.Y][position.X] = position;

        return string.Join(Environment.NewLine, grid.Select(x => string.Join(string.Empty, x.Select(y => GetPositionChar(y)))));
    }

    /// <summary>
    /// Parses a horizontal rock structure.
    /// </summary>
    /// <param name="first">Left scan value.</param>
    /// <param name="second">Right scan value.</param>
    /// <returns>The positions of the rock structure.</returns>
    private static IEnumerable<Position> ParseHorizontalStructure(decimal first, decimal second)
    {
        var (firstInteger, firstDecimal) = SplitIntoIntegerAndFraction(first);
        var (secondInteger, _) = SplitIntoIntegerAndFraction(second);
        var difference = firstInteger - secondInteger;

        while (difference is not 0)
        {
            yield return new(Content.Rock, firstInteger - difference, firstDecimal);
            difference += (difference is < 0) ? 1 : -1;
        }
    }

    /// <summary>
    /// Parses a vertical rock structure.
    /// </summary>
    /// <param name="first">Left scan value.</param>
    /// <param name="second">Right scan value.</param>
    /// <returns>The positions of the rock structure.</returns>
    private static IEnumerable<Position> ParseVerticalStructure(decimal first, decimal second)
    {
        var (firstInteger, firstDecimal) = SplitIntoIntegerAndFraction(first);
        var (_, secondDecimal) = SplitIntoIntegerAndFraction(second);
        var decimalDifference = Math.Max(firstDecimal, secondDecimal) - Math.Min(firstDecimal, secondDecimal);

        while (decimalDifference >= 0)
        {
            yield return new(Content.Rock, firstInteger, Math.Min(firstDecimal, secondDecimal) + decimalDifference);
            decimalDifference -= 1;
        }
    }

    /// <summary>
    /// Splits a decimal number into its integer and fractional parts.
    /// </summary>
    /// <param name="number">The number to be split.</param>
    /// <returns>A tuple with the integer part and the fractional part of the <paramref name="number"/>.</returns>
    private static (int Integer, int Fraction) SplitIntoIntegerAndFraction(decimal number)
    {
        var integer = (int)number;
        var decimalPart = decimal.GetBits(number - integer)[0]; // First index always contains the decimal part for values < 1 and > -1

        return (integer, decimalPart);
    }

    /// <summary>
    /// Gets the visual representation of a given position.
    /// </summary>
    /// <param name="position">The position to be represented.</param>
    /// <returns>A character that represents the position.</returns>
    /// <exception cref="UnreachableException">Occurs when the type of the position is not recognized.</exception>
    private static char GetPositionChar(Position position)
    {
        return position.Content switch
        {
            Content.Air => '.',
            Content.Sand => 'o',
            Content.Rock => '#',
            _ => throw new UnreachableException($"Could not parse position of type {position.Content}.")
        };
    }
}
