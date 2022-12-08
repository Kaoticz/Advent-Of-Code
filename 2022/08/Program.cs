using _08.Models;
using System.Collections.Immutable;

namespace _08;

internal sealed class Program
{
    private readonly static string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var treeMap = (await File.ReadAllLinesAsync(_inputLocation))
            .Select(x => x.Select(y => int.Parse(y.ToString())).ToImmutableArray() as IReadOnlyList<int>)
            .ToImmutableArray();

        var result = treeMap
            .SelectMany((treeLine, rowIndex) => treeLine.Select((_, columnIndex) => ParseTree(treeMap, rowIndex, columnIndex)))
            .ToImmutableArray();

        Console.WriteLine($"First answer: {result.Count(x => x.IsVisibleFromEdge)}");
        Console.WriteLine($"Second answer: {result.Max(x => x.ScenicScore)}");
    }

    /// <summary>
    /// Parses a tree from raw input data.
    /// </summary>
    /// <param name="treeMap">The raw input map.</param>
    /// <param name="treeRowIndex">The row index of the tree.</param>
    /// <param name="treeColumnIndex">The column index of the tree.</param>
    /// <returns>A parsed <see cref="Tree"/>.</returns>
    private static Tree ParseTree(IReadOnlyList<IReadOnlyList<int>> treeMap, in int treeRowIndex, in int treeColumnIndex)
    {
        var top = CountTreesTop(treeMap, treeRowIndex, treeColumnIndex);
        var left = CountTreesLeft(treeMap, treeRowIndex, treeColumnIndex);
        var bottom = CountTreesBottom(treeMap, treeRowIndex, treeColumnIndex);
        var right = CountTreesRight(treeMap, treeRowIndex, treeColumnIndex);

        return new(
            top.Item1 * left.Item1 * bottom.Item1 * right.Item1,
            top.Item2 || left.Item2 || bottom.Item2 || right.Item2
        );
    }

    /// <summary>
    /// Counts how many trees are visible to the top of a specific tree.
    /// </summary>
    /// <param name="treeMap">The tree map.</param>
    /// <param name="treeRowIndex">The row index of the tree.</param>
    /// <param name="treeColumnIndex">The column index of the tree.</param>
    /// <returns>
    /// A tuple with the amount of trees visible to the top, and a boolean
    /// indicating whether the tree is visible from the edge of the map.
    /// </returns>
    private static (int, bool) CountTreesTop(IReadOnlyList<IReadOnlyList<int>> treeMap, in int treeRowIndex, in int treeColumnIndex)
    {
        var currentTree = treeMap[treeRowIndex][treeColumnIndex];
        var isVisible = IsEdgeTree(treeMap, treeRowIndex, treeColumnIndex);
        var score = 0;

        for (var rowScout = treeRowIndex - 1; rowScout >= 0; rowScout--)
        {
            var nextTree = treeMap[rowScout][treeColumnIndex];
            score++;

            if (currentTree <= nextTree)
                break;
            else if (rowScout is 0)
                isVisible = true;
        }

        return (score, isVisible);
    }

    /// <summary>
    /// Counts how many trees are visible to the left of a specific tree.
    /// </summary>
    /// <param name="treeMap">The tree map.</param>
    /// <param name="treeRowIndex">The row index of the tree.</param>
    /// <param name="treeColumnIndex">The column index of the tree.</param>
    /// <returns>
    /// A tuple with the amount of trees visible to the left, and a boolean
    /// indicating whether the tree is visible from the edge of the map.
    /// </returns>
    private static (int, bool) CountTreesLeft(IReadOnlyList<IReadOnlyList<int>> treeMap, in int treeRowIndex, in int treeColumnIndex)
    {
        var currentTree = treeMap[treeRowIndex][treeColumnIndex];
        var isVisible = IsEdgeTree(treeMap, treeRowIndex, treeColumnIndex);
        var score = 0;

        for (var columnScout = treeColumnIndex - 1; columnScout >= 0; columnScout--)
        {
            var nextTree = treeMap[treeRowIndex][columnScout];
            score++;

            if (currentTree <= nextTree)
                break;
            else if (columnScout is 0)
                isVisible = true;
        }

        return (score, isVisible);
    }

    /// <summary>
    /// Counts how many trees are visible to the bottom of a specific tree.
    /// </summary>
    /// <param name="treeMap">The tree map.</param>
    /// <param name="treeRowIndex">The row index of the tree.</param>
    /// <param name="treeColumnIndex">The column index of the tree.</param>
    /// <returns>
    /// A tuple with the amount of trees visible to the bottom, and a boolean
    /// indicating whether the tree is visible from the edge of the map.
    /// </returns>
    private static (int, bool) CountTreesBottom(IReadOnlyList<IReadOnlyList<int>> treeMap, in int treeRowIndex, in int treeColumnIndex)
    {
        var currentTree = treeMap[treeRowIndex][treeColumnIndex];
        var isVisible = IsEdgeTree(treeMap, treeRowIndex, treeColumnIndex);
        var score = 0;

        for (var rowScout = treeRowIndex + 1; rowScout < treeMap.Count; rowScout++)
        {
            var nextTree = treeMap[rowScout][treeColumnIndex];
            score++;

            if (currentTree <= nextTree)
                break;
            else if (rowScout == treeMap.Count - 1)
                isVisible = true;
        }

        return (score, isVisible);
    }

    /// <summary>
    /// Counts how many trees are visible to the right of a specific tree.
    /// </summary>
    /// <param name="treeMap">The tree map.</param>
    /// <param name="treeRowIndex">The row index of the tree.</param>
    /// <param name="treeColumnIndex">The column index of the tree.</param>
    /// <returns>
    /// A tuple with the amount of trees visible to the right, and a boolean
    /// indicating whether the tree is visible from the edge of the map.
    /// </returns>
    private static (int, bool) CountTreesRight(IReadOnlyList<IReadOnlyList<int>> treeMap, in int treeRowIndex, in int treeColumnIndex)
    {
        var currentTree = treeMap[treeRowIndex][treeColumnIndex];
        var isVisible = IsEdgeTree(treeMap, treeRowIndex, treeColumnIndex);
        var score = 0;

        for (var columnScout = treeColumnIndex + 1; columnScout < treeMap[treeRowIndex].Count; columnScout++)
        {
            var nextTree = treeMap[treeRowIndex][columnScout];
            score++;

            if (currentTree <= nextTree)
                break;
            else if (columnScout == treeMap[treeRowIndex].Count - 1)
                isVisible = true;
        }

        return (score, isVisible);
    }

    /// <summary>
    /// Checks if the tree at the <paramref name="treeRowIndex"/> and <paramref name="treeColumnIndex"/>
    /// is at the edge of the <paramref name="treeMap"/>.
    /// </summary>
    /// <param name="treeMap">The tree map.</param>
    /// <param name="treeRowIndex">The row index of the tree.</param>
    /// <param name="treeColumnIndex">The column index of the tree.</param>
    /// <returns><see langword="true"/> if the tree is at the edge, <see langword="false"/> otherwise.</returns>
    private static bool IsEdgeTree(IReadOnlyList<IReadOnlyList<int>> treeMap, in int treeRowIndex, in int treeColumnIndex)
        => treeRowIndex is 0 || treeRowIndex == treeMap.Count - 1 || treeColumnIndex is 0 || treeColumnIndex == treeMap[treeRowIndex].Count - 1;
}