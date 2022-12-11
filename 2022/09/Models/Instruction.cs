using _09.Enums;

namespace _09.Models;

/// <summary>
/// Represents a simulation instruction for the movement of the rope.
/// </summary>
/// <param name="Direction">The direction the rope is supposed to move.</param>
/// <param name="Distance">How far the rope should move.</param>
internal record struct Instruction(Direction Direction, int Distance);
