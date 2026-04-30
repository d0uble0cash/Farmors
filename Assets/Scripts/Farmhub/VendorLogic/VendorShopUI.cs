using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorShopUI : MonoBehaviour
{
    [Header("Shop Logic")]
    [SerializeField] private VendorShop vendorShop;

    [Header("Generated Button Roots")]
    [SerializeField] private Transform buyContentRoot;
    [SerializeField] private Transform sellContentRoot;

    [Header("Button Prefab")]
    [SerializeField] private Button buttonPrefab;

    [Header("Optional Status / Inspector UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemStatsText;
    [SerializeField] private TMP_Text itemCostText;

    [Header("Optional Panels")]
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private GameObject sellPanel;

    [Header("Sell Settings")]
    [SerializeField] private int sellQuantityPerClick = 1;
    [SerializeField] private bool refreshOnEnable = true;
    [SerializeField] private bool startOnBuyTab = true;

    private readonly List<GameObject> spawnedBuyButtons = new();
    private readonly List<GameObject> spawnedSellButtons = new();

    private bool showingBuyTab = true;

    private void OnEnable()
    {
        showingBuyTab = startOnBuyTab;
        RefreshUI();
    }

    public void RefreshUI()
    {
        ClearSpawnedButtons();
        UpdateTabVisibility();

        BuildBuyButtons();
        BuildSellButtons();

        ClearInspector();

        if (statusText != null)
            statusText.text = string.Empty;
    }

    public void ShowBuyTab()
    {
        showingBuyTab = true;
        RefreshUI();
    }

    public void ShowSellTab()
    {
        showingBuyTab = false;
        RefreshUI();
    }

    private void UpdateTabVisibility()
    {
        if (buyPanel != null)
            buyPanel.SetActive(showingBuyTab);

        if (sellPanel != null)
            sellPanel.SetActive(!showingBuyTab);
    }

    private void BuildBuyButtons()
    {
        if (vendorShop == null || buyContentRoot == null || buttonPrefab == null)
            return;

        IReadOnlyList<VendorOffer> offers = vendorShop.Offers;

        for (int i = 0; i < offers.Count; i++)
        {
            VendorOffer offer = offers[i];
            if (offer == null || !offer.IsValid())
                continue;

            Button buttonInstance = Instantiate(buttonPrefab, buyContentRoot);
            spawnedBuyButtons.Add(buttonInstance.gameObject);

            TMP_Text label = buttonInstance.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                string itemName = offer.Item != null ? offer.Item.DisplayName : "Unknown Item";
                string currencyName = GetCurrencyDisplayName();
                label.text = $"{itemName} x{offer.Quantity} - {offer.Price} {currencyName}";
            }

            buttonInstance.onClick.RemoveAllListeners();
            buttonInstance.onClick.AddListener(() =>
            {
                ShowOfferInInspector(offer);

                bool success = vendorShop.Buy(offer);

                if (statusText != null)
                {
                    statusText.text = success
                        ? $"Bought {offer.Quantity}x {offer.Item.DisplayName}"
                        : "Not enough currency.";
                }

                RefreshUI();
            });
        }
    }

    private void BuildSellButtons()
    {
        if (vendorShop == null || sellContentRoot == null || buttonPrefab == null || GameState.I == null)
            return;

        IReadOnlyDictionary<string, int> items = GameState.I.PlayerInventory.Items;
        ItemDefinition currencyItem = vendorShop.GetCurrencyItem();

        foreach (var pair in items)
        {
            string itemId = pair.Key;
            int ownedAmount = pair.Value;

            if (ownedAmount <= 0)
                continue;

            if (currencyItem != null && itemId == currencyItem.Id)
                continue;

            ItemDefinition itemDef = ItemDatabase.itemDatabase != null
                ? ItemDatabase.itemDatabase.GetItemById(itemId)
                : null;

            if (itemDef == null)
                continue;

            int quantityToSell = Mathf.Min(sellQuantityPerClick, ownedAmount);
            int sellPricePerItem = vendorShop.GetSellPrice(itemDef);

            if (sellPricePerItem <= 0)
                continue;

            int totalSellPrice = sellPricePerItem * quantityToSell;

            Button buttonInstance = Instantiate(buttonPrefab, sellContentRoot);
            spawnedSellButtons.Add(buttonInstance.gameObject);

            TMP_Text label = buttonInstance.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.text = $"{itemDef.DisplayName} (x{ownedAmount})";
            }

            buttonInstance.onClick.RemoveAllListeners();
            buttonInstance.onClick.AddListener(() =>
            {
                ShowSellItemInInspector(itemDef, ownedAmount, totalSellPrice, quantityToSell);

                bool success = vendorShop.Sell(itemDef, quantityToSell);

                if (statusText != null)
                {
                    statusText.text = success
                        ? $"Sold {quantityToSell}x {itemDef.DisplayName}"
                        : $"Could not sell {itemDef.DisplayName}.";
                }

                RefreshUI();
            });
        }
    }

    private void ShowOfferInInspector(VendorOffer offer)
    {
        if (offer == null || offer.Item == null)
            return;

        if (itemNameText != null)
            itemNameText.text = offer.Item.DisplayName;

        if (itemStatsText != null)
            itemStatsText.text = $"Stock Item\nQuantity: {offer.Quantity}";

        if (itemCostText != null)
            itemCostText.text = $"Buy Cost: {offer.Price} {GetCurrencyDisplayName()}";
    }

    private void ShowSellItemInInspector(ItemDefinition itemDef, int ownedAmount, int totalSellPrice, int quantityToSell)
    {
        if (itemDef == null)
            return;

        if (itemNameText != null)
            itemNameText.text = itemDef.DisplayName;

        if (itemStatsText != null)
            itemStatsText.text = $"Owned: {ownedAmount}\nSell Qty: {quantityToSell}";

        if (itemCostText != null)
            itemCostText.text = $"Sell Value: +{totalSellPrice} {GetCurrencyDisplayName()}";
    }

    private void ClearInspector()
    {
        if (itemNameText != null)
            itemNameText.text = string.Empty;

        if (itemStatsText != null)
            itemStatsText.text = string.Empty;

        if (itemCostText != null)
            itemCostText.text = string.Empty;
    }

    private string GetCurrencyDisplayName()
    {
        if (vendorShop == null)
            return "Currency";

        ItemDefinition currencyItem = vendorShop.GetCurrencyItem();
        if (currencyItem == null)
            return "Currency";

        return currencyItem.DisplayName;
    }

    private void ClearSpawnedButtons()
    {
        for (int i = 0; i < spawnedBuyButtons.Count; i++)
        {
            if (spawnedBuyButtons[i] != null)
                Destroy(spawnedBuyButtons[i]);
        }
        spawnedBuyButtons.Clear();

        for (int i = 0; i < spawnedSellButtons.Count; i++)
        {
            if (spawnedSellButtons[i] != null)
                Destroy(spawnedSellButtons[i]);
        }
        spawnedSellButtons.Clear();
    }
}