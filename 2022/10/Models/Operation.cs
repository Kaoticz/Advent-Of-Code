using _10.Enums;

namespace _10.Models;

/// <summary>
/// Represents a CPU operation.
/// </summary>
/// <param name="Type">The type of the operation.</param>
/// <param name="ToAdd">The value to be added to X register.</param>
internal record struct Operation(OperationType Type, int ToAdd);
