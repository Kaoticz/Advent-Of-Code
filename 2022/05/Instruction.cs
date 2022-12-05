namespace _05;

internal readonly struct Instruction
{
    public int Amount { get; }
    public int FromStack { get; }
    public int ToStack { get; }

    public Instruction(int amount, int fromStack, int toStack)
    {
        Amount = amount;
        FromStack = fromStack;
        ToStack = toStack;
    }
}
