using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        RefreshText();
    }
    private void RefreshText()
    {
        if (text == null) 
        {
            return;
        }

        if (GameState.I == null)
        {
            text.text = "Inventory:\nUnavailable";
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
                newText += $"{item.Key}: {item.Value}\n";
            }
        }
        text.text = newText;
    }
}