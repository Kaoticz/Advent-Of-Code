namespace _12.Models;

/// <summary>
/// Represents a <see cref="Node"/> linked to another <see cref="Node"/>.
/// </summary>
/// <param name="Parent">The <see cref="Node"/> that points to this <see cref="Node"/>.</param>
/// <param name="Node">The <see cref="Node"/> being pointed to.</param>
internal sealed record LinkedNode(LinkedNode? Parent, Node Node);