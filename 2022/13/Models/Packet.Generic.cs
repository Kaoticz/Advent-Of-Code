namespace _13.Models;

/// <summary>
/// Represents a packet and its held value.
/// </summary>
/// <param name="Value">The value contained in this packet.</param>
/// <typeparam name="T">The type of the value held by this packet.</typeparam>
internal sealed record Packet<T>(T Value) : Packet;