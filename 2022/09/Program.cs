using _09.Enums;
using _09.Models;
using System.Collections.Immutable;
using System.Diagnostics;

namespace _09;

internal sealed class Program
{
    private readonly static string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var input = await File.ReadAllLinesAsync(_inputLocation);
        var instructions = input
            .Select(ParseInstruction)
            .ToImmutableArray();

        var result1 = RunSimulation(instructions, 2);
        var result2 = RunSimulation(instructions, 10);

        Console.WriteLine($"First answer: {result1.Count}");
        Console.WriteLine($"Second answer: {result2.Count}");
    }

    /// <summary>
    /// Parses an instruction from raw input data.
    /// </summary>
    /// <param name="instruction">The raw instruction.</param>
    /// <returns>A parsed <see cref="Instruction"/>.</returns>
    private static Instruction ParseInstruction(string instruction)
        => new(ParseDirection(instruction.AsSpan()[0]), int.Parse(instruction.AsSpan()[1..]));

    /// <summary>
    /// Parses a direction from raw input data.
    /// </summary>
    /// <param name="direction">The direction to be parsed.</param>
    /// <returns>A parsed <see cref="Direction"/>.</returns>
    /// <exception cref="UnreachableException">Occurs when the direction is not recognized.</exception>
    private static Direction ParseDirection(char direction)
    {
        return direction switch
        {
            'U' => Direction.Up,
            'D' => Direction.Down,
            'L' => Direction.Left,
            'R' => Direction.Right,
            _ => throw new UnreachableException($"Direction of type '{direction}' is unknown.")
        };
    }

    /// <summary>
    /// Executes the rope simulation.
    /// </summary>
    /// <param name="instructions">The instructions for the movement of the rope.</param>
    /// <param name="ropeLength">The length of the rope.</param>
    /// <returns>A collection of unique positions visited by the tail of the rope.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="ropeLength"/> is less than 2.</exception>
    private static IReadOnlySet<Position> RunSimulation(IEnumerable<Instruction> instructions, int ropeLength)
    {
        if (ropeLength < 2)
            throw new ArgumentException("The rope cannot have a length smaller than 2.", nameof(ropeLength));

        var uniquePositions = new HashSet<Position>();
        var knots = Enumerable.Range(0, ropeLength)
            .Select(_ => new Position(0, 0))
            .ToArray() as IList<Position>;

        foreach (var instruction in instructions)
        {
            for (var amount = instruction.Distance; amount > 0; amount--)
            {
                knots = MoveRope(knots, instruction.Direction);
                uniquePositions.Add(knots[knots.Count - 1]);
            }
        }

        return uniquePositions;
    }

    /// <summary>
    /// Moves the rope to the specified <paramref name="direction"/>.
    /// </summary>
    /// <param name="rope">The rope to move.</param>
    /// <param name="direction">The direction to move the rope to.</param>
    /// <returns>The rope in its new position.</returns>
    private static IList<Position> MoveRope(IList<Position> rope, Direction direction)
    {
        // Move the head
        rope[0] = MoveHeadKnot(rope[0], direction);

        // Then the tail
        for (var index = 1; index < rope.Count; index++)
            rope[index] = MoveTailKnot(rope[index - 1], rope[index]);

        return rope;
    }

    /// <summary>
    /// Moves a head knot to the specified <paramref name="direction"/>.
    /// </summary>
    /// <param name="headKnot">The head knot.</param>
    /// <param name="direction">The direction to move the knot to.</param>
    /// <returns>The new position of the head knot.</returns>
    /// <exception cref="UnreachableException">Occurs when the direction is not recognized.</exception>
    private static Position MoveHeadKnot(Position headKnot, Direction direction)
    {
        return direction switch
        {
            Direction.Up => headKnot with { X = headKnot.X - 1 },
            Direction.Down => headKnot with { X = headKnot.X + 1 },
            Direction.Left => headKnot with { Y = headKnot.Y - 1 },
            Direction.Right => headKnot with { Y = headKnot.Y + 1 },
            _ => throw new UnreachableException($"Direction of type '{direction}' is unknown.")
        };
    }

    /// <summary>
    /// Moves a tail knot towards the specified head knot.
    /// </summary>
    /// <param name="headKnot">The head knot.</param>
    /// <param name="tailKnot">The tail knot to be moved.</param>
    /// <returns>The new position of the tail knot.</returns>
    private static Position MoveTailKnot(Position headKnot, Position tailKnot)
    {
        // If head and tail are adjacent or overlapping, don't move the tail
        if (headKnot == tailKnot || IsKnotAdjacent(headKnot, tailKnot))
            return tailKnot;
        else if (headKnot.X == tailKnot.X)                  // Vertical movements
        {
            return (headKnot.Y > tailKnot.Y)
                ? new(headKnot.X, headKnot.Y - 1)        // Move tail to the right
                : new(headKnot.X, headKnot.Y + 1);       // Move tail to the left
        }
        else if (headKnot.Y == tailKnot.Y)
        {
            return (headKnot.X > tailKnot.X)
                ? new(headKnot.X - 1, headKnot.Y)        // Move tail downwards
                : new(headKnot.X + 1, headKnot.Y);       // Move tail upwards
        }
        else if (headKnot.X > tailKnot.X)                   // Diagonal movements
        {
            return (headKnot.Y > tailKnot.Y)
                ? new(tailKnot.X + 1, tailKnot.Y + 1)    // Move tail to bottom right
                : new(tailKnot.X + 1, tailKnot.Y - 1);   // Move tail to bottom left
        }
        else
        {
            return (headKnot.Y > tailKnot.Y)
                ? new(tailKnot.X - 1, tailKnot.Y + 1)    // Move tail to top right
                : new(tailKnot.X - 1, tailKnot.Y - 1);   // Move tail to top left
        }
    }

    /// <summary>
    /// Checks if two knots are next to each other.
    /// </summary>
    /// <param name="headKnot">The head knot.</param>
    /// <param name="tailKnot">The tail knot.</param>
    /// <returns><see langword="true"/> if the knots are next to each other, <see langword="false"/> otherwise.</returns>
    private static bool IsKnotAdjacent(Position headKnot, Position tailKnot)
    {
        return tailKnot.X <= headKnot.X + 1 && tailKnot.X >= headKnot.X - 1
            && tailKnot.Y <= headKnot.Y + 1 && tailKnot.Y >= headKnot.Y - 1;
    }
}