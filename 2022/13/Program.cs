using System.Text.Json;
using System.Diagnostics;
using System.Collections.Immutable;
using _13.Models;

namespace _13;

internal sealed class Program
{
    private static readonly string _inputLocation = Path.Combine("Data", "input.txt");

    private static async Task Main()
    {
        var firstDividerPacket = ParsePacket(JsonDocument.Parse("[[2]]"));
        var secondDividerPacket = ParsePacket(JsonDocument.Parse("[[6]]"));

        var packets = (await File.ReadAllLinesAsync(_inputLocation))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => ParsePacket(JsonDocument.Parse(x)))
            .ToImmutableArray();

        var result1 = packets
            .Chunk(2)
            .Select((packet, index) => (Packet: packet, PairIndex: index + 1))
            .Where(x => ArePacketsValid(x.Packet[0], x.Packet[1]) is true)
            .Sum(x => x.PairIndex);

        var result2 = packets
            .Append(firstDividerPacket)
            .Append(secondDividerPacket)
            .Order(Comparer<Packet?>.Create(ComparePackets))
            .ToImmutableArray();

        Console.WriteLine($"First answer: {result1}");
        Console.WriteLine($"Second answer: {(result2.IndexOf(firstDividerPacket) + 1) * (result2.IndexOf(secondDividerPacket) + 1)}");
    }

    /// <summary>
    /// Parses a packet.
    /// </summary>
    /// <param name="packet">The json document representing the packet.</param>
    /// <returns>A parsed <see cref="Packet"/>.</returns>
    private static Packet ParsePacket(JsonDocument packet)
       => ParsePacket(packet.RootElement);

    /// <summary>
    /// Parses a packet.
    /// </summary>
    /// <param name="packet">The json element representing the packet.</param>
    /// <returns>A parsed <see cref="Packet"/>.</returns>
    private static Packet ParsePacket(JsonElement packet)
    {
        if (packet.ValueKind is JsonValueKind.Number)
            return new Packet<int>(packet.GetInt32());

        var result = packet.EnumerateArray()
            .Select(ParsePacket)
            .ToImmutableArray();

        return new Packet<IReadOnlyList<Packet>>(result);
    }

    /// <summary>
    /// Provides a <see cref="Comparison{T}"/> implementation for <see cref="Packet"/> comparisons.
    /// </summary>
    /// <param name="first">The first packet.</param>
    /// <param name="second">The second packet.</param>
    /// <returns>-1 if the packets are valid, 0 if the packets are equivalent, and 1 if the packets are invalid.</returns>
    public static int ComparePackets(Packet? first, Packet? second)
    {
        return ArePacketsValid(first, second) switch
        {
            true => -1,
            null => 0,
            false => 1
        };
    }

    /// <summary>
    /// Checks if two packets are valid.
    /// </summary>
    /// <param name="first">The first packet.</param>
    /// <param name="second">The second packet.</param>
    /// <returns>
    /// <see langword="true"/> if the packets are valid, <see langword="null"/>
    /// if they are equivalent, and <see langword="false"/> if they are invalid.
    /// </returns>
    /// <exception cref="ArgumentException">Occurs when the type of one of the packets is not recognized.</exception>
    public static bool? ArePacketsValid(Packet? first, Packet? second)
    {
        return (first, second) switch
        {
            (Packet, null) => false,
            (null, Packet) => true,
            (Packet<int> x, Packet<int> y) when x.Value == y.Value => null,
            (Packet<int> x, Packet<int> y) => x.Value < y.Value,
            (Packet<IReadOnlyList<Packet>> x, Packet<int> y) => ArePacketsValid(x, new Packet<IReadOnlyList<Packet>>(new[] { y })),
            (Packet<int> x, Packet<IReadOnlyList<Packet>> y) => ArePacketsValid(new Packet<IReadOnlyList<Packet>>(new[] { x }), y),
            (Packet<IReadOnlyList<Packet>> x, Packet<IReadOnlyList<Packet>> y) => ComparePacketCollections(x.Value, y.Value),
            _ => throw new ArgumentException("Packets of invalid type were received.")
        };
    }

    /// <summary>
    /// Compares two packet collections.
    /// </summary>
    /// <param name="first">The first packet.</param>
    /// <param name="second">The second packet.</param>
    /// <returns>
    /// <see langword="true"/> if the packets are valid, <see langword="null"/>
    /// if they are equivalent, and <see langword="false"/> if they are invalid.
    /// </returns>
    private static bool? ComparePacketCollections(IReadOnlyList<Packet> first, IReadOnlyList<Packet> second)
    {
        bool? isPacketValid = null;
        var largest = Math.Max(first.Count, second.Count);

        for (var index = 0; index < largest && isPacketValid is null; index++)
        {
            var firstNode = (index < first.Count)
                ? first[index]
                : default;

            var secondNode = (index < second.Count)
                ? second[index]
                : default;

            isPacketValid = ArePacketsValid(firstNode, secondNode);
        }

        return isPacketValid;
    }
}