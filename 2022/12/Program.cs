using System.Collections.Immutable;
using _12.Models;

namespace _12;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var map = (await File.ReadAllLinesAsync(_inputLocation))
            .SelectMany((row, rowIndex) => row.Select((letter, columnIndex) => new Node(letter, columnIndex, rowIndex)))
            .ToImmutableArray();

        var result1 = ExecuteDijkstra(map, 'S', 'E', Node.CanMoveDropping);
        var result2 = ExecuteDijkstra(map, 'E', 'a', Node.CanMoveClimbing);

        Console.WriteLine($"First answer: {result1.Count}");
        Console.WriteLine($"Second answer: {result2.Count}");
    }

    /// <summary>
    /// Executes the pathfinding Dijkstra algorithm.
    /// </summary>
    /// <param name="grid">The grid of nodes.</param>
    /// <param name="startChar">The starting char in the grid.</param>
    /// <param name="endChar">The destination char in the grid.</param>
    /// <param name="movementRules">A function that defines the rules for movement between nodes.</param>
    /// <returns>The shortest path between the specified nodes.</returns>
    /// <remarks>Pathfinding starts from the first <paramref name="startChar"/> found in the <paramref name="grid"/>.</remarks>
    /// <exception cref="InvalidOperationException">
    /// Occurs when <paramref name="startChar"/> is not found in the <paramref name="grid"/> or when
    /// no valid path to <paramref name="endChar"/> is found.
    /// </exception>
    private static IReadOnlyList<Node> ExecuteDijkstra(IReadOnlyList<Node> grid, char startChar, char endChar, Func<Node, Node, bool> movementRules)
    {
        var visited = new LinkedNode(null, grid.First(x => x.Value == startChar));
        var visitedNodes = new HashSet<Node>();
        var candidateNodes = new Queue<LinkedNode>();
        candidateNodes.Enqueue(visited);

        while (visited.Node.Value != endChar)
        {
            // Take node from the queue
            visited = candidateNodes.Dequeue();

            // If node has not been visited, survey its candidates
            if (visitedNodes.Contains(visited.Node))
                continue;

            foreach (var candidate in FindCandidateNodes(grid, visited, movementRules))
                candidateNodes.Enqueue(candidate);

            visitedNodes.Add(visited.Node);
        }

        return TraversePath(visited)
            .Reverse()
            .ToImmutableArray();
    }

    /// <summary>
    /// Finds nodes suitable for movement from the <paramref name="parent"/> node.
    /// </summary>
    /// <param name="grid">The grid of nodes.</param>
    /// <param name="parent">The node where movement is coming from.</param>
    /// <param name="movementRules">A function that defines the rules for the movement itself.</param>
    /// <returns>The candidate nodes.</returns>
    private static IEnumerable<LinkedNode> FindCandidateNodes(IReadOnlyList<Node> grid, LinkedNode parent, Func<Node, Node, bool> movementRules)
    {
        return grid
            .Where(destination => destination != parent.Node && movementRules(parent.Node, destination))
            .Select(node => new LinkedNode(parent, node));
    }

    /// <summary>
    /// Traces the path from the specified node all the way to the first node.
    /// </summary>
    /// <param name="end">The node to trace from.</param>
    /// <returns>The entire path up to the <paramref name="end"/> node, reversed.</returns>
    private static IEnumerable<Node> TraversePath(LinkedNode end)
    {
        while (end.Parent is not null)
        {
            yield return end.Node;
            end = end.Parent;
        }
    }
}