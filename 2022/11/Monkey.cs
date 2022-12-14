namespace _11;

/// <summary>
/// Monke
/// </summary>
internal sealed class Monkey
{
    private readonly List<long> _items;
    private readonly Func<long, long> _inspection;
    private readonly Func<long, int> _chooseNextMonkey;

    /// <summary>
    /// How many items this monkey is currently holding.
    /// </summary>
    public int ItemsInPossession
        => _items.Count;

    /// <summary>
    /// How many times this monkey has thrown an item.
    /// </summary>
    public long ThrowAmount { get; private set; }

    /// <summary>
    /// The identification of this monkey.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The divisor compatible with all of this monkey's inspections.
    /// </summary>
    public long Divisor { get; }

    /// <summary>
    /// Initializes a monkey.
    /// </summary>
    /// <param name="id">The identification of the monkey in the group.</param>
    /// <param name="divisor">The divisor compatible with the worry levels generated by this monkey's <paramref name="inspection"/>.</param>
    /// <param name="items">The itens this monkey initially starts with.</param>
    /// <param name="inspection">A function that takes the value of the item and adds a worry level to it.</param>
    /// <param name="chooseNextMonkey">A function that defines the ID of the monkeys this monkey can throw items to.</param>
    public Monkey(int id, long divisor, IEnumerable<long> items, Func<long, long> inspection, Func<long, int> chooseNextMonkey)
    {
        Id = id;
        Divisor = divisor;
        _items = new(items);
        _inspection = inspection;
        _chooseNextMonkey = chooseNextMonkey;
    }

    /// <summary>
    /// Throws an item to the next monkey in the group.
    /// </summary>
    /// <param name="monkeys">The group of monkeys.</param>
    /// <param name="divisors">The divisors for the worry level.</param>
    /// <param name="worryReduceFactor">A factor that reduces the worry levels when the monkey gets bored with the inspected item.</param>
    /// <returns>The monkey that received the item.</returns>
    /// <exception cref="InvalidOperationException">Occurs when this monkey has no items to throw.</exception>
    /// <exception cref="ArgumentException">Occurs when <paramref name="worryReduceFactor"/> is zero.</exception>
    public Monkey ThrowItem(IReadOnlyList<Monkey> monkeys, long divisors, long worryReduceFactor = 1)
    {
        if (_items.Count is 0)
            throw new InvalidOperationException($"Monkey {Id} has no items to throw!");
        else if (worryReduceFactor is 0)
            throw new ArgumentException("Cannot be zero.", nameof(worryReduceFactor));

        // Monkey performs inspection and raises worry level
        var worry = _inspection(_items[0]) / worryReduceFactor;

        // Monkey chooses the next monkey to throw the item to
        var monkeyId = _chooseNextMonkey(worry);

        // Throw the item to the next monkey
        _items.RemoveAt(0);
        monkeys[monkeyId].CatchItem(worry % divisors);
        ThrowAmount++;

        return monkeys[monkeyId];
    }

    /// <summary>
    /// Catch an item/worry level.
    /// </summary>
    /// <param name="item">The item/worry level thrown at this monkey.</param>
    private void CatchItem(long item)
        => _items.Add(item);
}