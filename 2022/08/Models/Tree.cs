namespace _08.Models;

/// <summary>
/// Represents a tree.
/// </summary>
/// <param name="ScenicScore">The scenic score of the tree.</param>
/// <param name="IsVisibleFromEdge">Defines whether the tree is visible from the edge of the map.</param>
internal sealed record Tree(int ScenicScore, bool IsVisibleFromEdge);