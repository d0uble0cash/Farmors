using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private InventoryModel currentInventory;

    [Header("Refs")]
    public InventorySlot[] weaponitemSlots, farmitemSlots, materialitemTexts;

    [SerializeField] public GameObject sellCheckPanelMaterial;
    [SerializeField] private GameObject sellCheckPanel;
    [SerializeField] private TMP_InputField matInput;
    [SerializeField] private TMP_Text MaterialText;
    [SerializeField] private TMP_Text SellText;

    public void Show(InventoryModel inventory)
    {
        currentInventory = inventory;
        gameObject.SetActive(true);
        RefreshSlots();
    }

    public void UpdateUI()
    {
        RefreshSlots();
    }

    private void RefreshSlots()
    {
        if (GameState.I == null)
            return;

        ClearSlot(weaponitemSlots);
        ClearSlot(materialitemTexts);
        ClearSlot(farmitemSlots);

        var items = currentInventory != null
            ? currentInventory.Items
            : GameState.I.PlayerInventory.Items;

        if (items.Count == 0)
            return;

        foreach (var item in items)
        {
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);

            if (definition == null)
                continue;

            if (!definition.IsMaterial)
            {
                switch (definition.Id)
                {
                    case "shears":
                    case "pitchfork":
                        AddToFirstOpenSlot(farmitemSlots, definition);
                        break;

                    case "sword":
                    case "bow":
                    case "greatsword":
                    case "darksword":
                    case "crossbow":
                        AddToFirstOpenSlot(weaponitemSlots, definition);
                        break;
                }
            }
            else if (definition.Id != "gold")
            {
                AddMaterialToFirstOpenSlot(materialitemTexts, definition, item.Value);
            }
        }
    }

    private void AddToFirstOpenSlot(InventorySlot[] slots, ItemDefinition definition)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].isFull)
            {
                slots[i].addItem(definition);
                break;
            }
        }
    }

    private void AddMaterialToFirstOpenSlot(InventorySlot[] slots, ItemDefinition definition, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].isFull)
            {
                slots[i].addItemM(definition, amount);
                break;
            }
        }
    }

    public void SellSelectFirst()
    {
        InventorySlot selected = findSelected();

        if (selected == null)
            return;

        ItemDefinition definition = selected.itemInSlot;

        if (definition == null || definition.Id == "gold")
            return;

        if (definition.IsMaterial)
        {
            sellCheckPanelMaterial.SetActive(true);
            MaterialText.text = $"Sell {definition.DisplayName}?";
        }
        else
        {
            sellCheckPanel.SetActive(true);
            SellText.text = $"Sell {definition.DisplayName}?";
        }
    }

    public void SellSelected()
    {
        InventorySlot selected = findSelected();

        if (selected == null)
            return;

        ItemDefinition definition = selected.itemInSlot;

        if (definition == null)
            return;

        GameState.I.PlayerInventory.Add("gold", definition.ItemValue);
        GameState.I.PlayerInventory.TryRemove(definition.Id, 1);

        UpdateUI();
    }

    public void SellSelectedM()
    {
        InventorySlot selected = findSelected();

        if (selected == null)
            return;

        ItemDefinition definition = selected.itemInSlot;

        if (definition == null)
            return;

        int sellAmount = int.Parse(matInput.text);

        if (GameState.I.PlayerInventory.GetCount(definition.Id) < sellAmount)
            return;

        GameState.I.PlayerInventory.Add("gold", definition.ItemValue * sellAmount);
        GameState.I.PlayerInventory.TryRemove(definition.Id, sellAmount);

        UpdateUI();
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < weaponitemSlots.Length; i++)
            weaponitemSlots[i].isSelected = false;

        for (int i = 0; i < farmitemSlots.Length; i++)
            farmitemSlots[i].isSelected = false;

        for (int i = 0; i < materialitemTexts.Length; i++)
            materialitemTexts[i].isSelected = false;
    }

    private void ClearSlot(InventorySlot[] sentSlots)
    {
        for (int i = 0; i < sentSlots.Length; i++)
            sentSlots[i].clearSlot();
    }

    private InventorySlot findSelected()
    {
        InventorySlot selectedSlot = selectSort(weaponitemSlots);

        if (selectedSlot == null)
            selectedSlot = selectSort(materialitemTexts);

        if (selectedSlot == null)
            selectedSlot = selectSort(farmitemSlots);

        return selectedSlot;
    }

    private InventorySlot selectSort(InventorySlot[] sentSlot)
    {
        for (int i = 0; i < sentSlot.Length; i++)
        {
            if (sentSlot[i].isSelected)
                return sentSlot[i];
        }

        return null;
    }
}