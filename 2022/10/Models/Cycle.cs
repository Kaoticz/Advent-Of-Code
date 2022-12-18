using _10.Enums;

namespace _10.Models;

/// <summary>
/// Represents a CPU cycle.
/// </summary>
/// <param name="Id">The Id of the cycle.</param>
/// <param name="Type">The type of the operation that is being executed in this cycle.</param>
/// <param name="XValue">The current value in the X register.</param>
internal sealed record Cycle(int Id, OperationType Type, int XValue);
