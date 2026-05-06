public static class InventoryTransfer
{
    public static bool Transfer(
        InventoryModel from,
        InventoryModel to,
        string id,
        int amount)
    {
        if (from == null || to == null)
            return false;

        if (!from.Has(id, amount))
            return false;

        if (!from.TryRemove(id, amount))
            return false;

        if (!to.Add(id, amount))
        {
            from.Add(id, amount);
            return false;
        }

        return true;
    }
}