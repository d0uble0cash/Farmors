using System.Collections.Generic;
using UnityEngine;

public class VendorShop : MonoBehaviour
{
    [Header("Currency")]
    [SerializeField] private ItemDefinition currencyItem;
    [SerializeField] private int sellPriceDivisor = 2;

    [Header("Shop Offers")]
    [SerializeField] private List<VendorOffer> offers = new();

    public IReadOnlyList<VendorOffer> Offers => offers;

    public bool CanBuy(VendorOffer offer)
    {
        if (GameState.I == null || currencyItem == null || offer == null || !offer.IsValid())
            return false;

        return GameState.I.PlayerInventory.Has(currencyItem.Id, offer.Price);
    }

    public bool Buy(VendorOffer offer)
    {
        if (GameState.I == null || currencyItem == null || offer == null || !offer.IsValid())
            return false;

        var inventory = GameState.I.PlayerInventory;

        if (!inventory.TryRemove(currencyItem.Id, offer.Price))
            return false;

        inventory.Add(offer.Item.Id, offer.Quantity);
        return true;
    }

    public bool CanSell(ItemDefinition item, int quantity = 1)
    {
        if (GameState.I == null || currencyItem == null || item == null || quantity <= 0)
            return false;

        if (item.Id == currencyItem.Id)
            return false;

        return GameState.I.PlayerInventory.Has(item.Id, quantity);
    }

    public bool Sell(ItemDefinition item, int quantity = 1)
    {
        if (GameState.I == null || currencyItem == null || item == null || quantity <= 0)
            return false;

        if (item.Id == currencyItem.Id)
            return false;

        int sellPricePerItem = GetSellPrice(item);
        if (sellPricePerItem <= 0)
            return false;

        var inventory = GameState.I.PlayerInventory;

        if (!inventory.TryRemove(item.Id, quantity))
            return false;

        inventory.Add(currencyItem.Id, sellPricePerItem * quantity);
        return true;
    }

    public int GetSellPrice(ItemDefinition item)
    {
        if (item == null || item.ItemValue <= 0)
            return 0;

        return Mathf.Max(1, item.ItemValue / Mathf.Max(1, sellPriceDivisor));
    }

    public ItemDefinition GetCurrencyItem()
    {
        return currencyItem;
    }
}