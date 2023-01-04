using _21.Models.Abstractions;
using System.Diagnostics;

namespace _21.Models;

/// <summary>
/// Represents a monkey whose number depends on the value screamed by
/// two other monkeys after a mathematical operation.
/// </summary>
public sealed record MathMonkey : Monkey
{
    /// <summary>
    /// The operator of the mathematical operation
    /// to be performed by this monkey.
    /// </summary>
    public char Operator { get; }

    /// <summary>
    /// The operator that reverses the mathematical
    /// operation performed by this monkey.
    /// </summary>
    public char ReverseOperator { get; }

    /// <summary>
    /// The monkey that screams the left operand.
    /// </summary>
    public Monkey LeftMonkey { get; }

    /// <summary>
    /// The monkey that screams the right operand.
    /// </summary>
    public Monkey RightMonkey { get; }

    /// <summary>
    /// Initializes a monkey that performs a mathematical operation
    /// on the values screamed by two other monkeys.
    /// </summary>
    /// <param name="name">The name of the monkey.</param>
    /// <param name="operator">The operator of the mathematical operation that should be performed.</param>
    /// <param name="leftMonkey">The monkey that screams the left operand.</param>
    /// <param name="rightMonkey">The monkey that screams the right operand.</param>
    /// <exception cref="UnreachableException">Occurs when <paramref name="operator"/> is not recognized.</exception>
    public MathMonkey(string name, char @operator, Monkey leftMonkey, Monkey rightMonkey) : base(name)
    {
        Operator = @operator;
        LeftMonkey = leftMonkey;
        RightMonkey = rightMonkey;
        ReverseOperator = @operator switch
        {
            '+' => '-',
            '-' => '+',
            '*' => '/',
            '/' => '*',
            _ => throw new UnreachableException($"Operator of type {@operator} is not recognized.")
        };
    }

    /// <inheritdoc />
    public override long GetValue()
        => ExecuteOperation(LeftMonkey.GetValue(), RightMonkey.GetValue(), Operator);

    /// <summary>
    /// Executes a mathematical operation.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <param name="operator">The operator.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="UnreachableException">Occurs when <paramref name="operator"/> is not recognized.</exception>
    public static long ExecuteOperation(long left, long right, char @operator)
    {
        return @operator switch
        {
            '+' => left + right,
            '-' => left - right,
            '*' => left * right,
            '/' => left / right,
            _ => throw new UnreachableException($"Operator of type {@operator} is not recognized.")
        };
    }
}