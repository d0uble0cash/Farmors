using System.Collections.Generic;

public class InventoryModel
{
    private readonly Dictionary<string, int> items = new();
    public IReadOnlyDictionary<string, int> Items => items;

    private static bool VerifyValid(string id, int amount)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;

        if (amount <= 0) return false;
        return true;
    }
    public bool Add(string id, int amount)
    {
        if (!VerifyValid(id, amount)) return false;

        if (items.TryGetValue(id, out var current))
            items[id] = current + amount;
        else
            items[id] = amount;

        return true;
    }

    public bool TryRemove(string id, int amount)
    {
        if (!VerifyValid(id, amount)) return false;

        if (!items.TryGetValue(id, out var current))
            return false;

        if (current < amount)
            return false;

        current -= amount;

        if (current == 0)
            items.Remove(id);
        else
            items[id] = current;

        return true;
    }

    public int GetCount(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return 0;
        return items.TryGetValue(id, out var count) ? count : 0;
    }

    public bool Has(string id, int amount = 1)
    {
        if (!VerifyValid(id, amount)) return false;
        return GetCount(id) >= amount;
    }
}