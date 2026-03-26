using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text inventoryText;

    private void Awake()
    {
        if (inventoryText == null)
            inventoryText = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        RefreshText();
    }
    private void RefreshText()
    {
        if (inventoryText == null) 
        {
            return;
        }

        if (GameState.I == null)
        {
            inventoryText.text = "Inventory:\nUnavailable";
            return;
        }
        
        string newText = "Inventory:\n";
        var items = GameState.I.PlayerInventory.Items;

        if (items.Count == 0)
        {
            newText += "Empty";
        }
        else
        {
            foreach (var item in items)
            {
                ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
                newText += $"{definition?.DisplayName ?? item.Key}: {item.Value}\n";
            }
        }
        inventoryText.text = newText;
    }
}