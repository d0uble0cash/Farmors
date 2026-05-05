using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryCheat : MonoBehaviour
{
    [Header("Test Item")]
    [SerializeField] private ItemDefinition testItem;
    [SerializeField] private int addAmount = 5;

    void Update()
    {
        if (GameState.I == null)
            return;

        // Add item (press O)
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            if (testItem == null)
            {
                Debug.LogWarning("Test Item not assigned.");
                return;
            }

            GameState.I.PlayerInventory.Add(testItem.Id, addAmount);
            Debug.Log($"Added {addAmount}x {testItem.DisplayName}");
        }

        // Print inventory (press P)
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            PrintInventory();
        }

        // Clear inventory (press K)
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            Debug.Log("Clearing inventory...");
            ClearInventory();
        }
    }

    private void PrintInventory()
    {
        Debug.Log("=== INVENTORY ===");

        var items = GameState.I.PlayerInventory.Items;

        if (items.Count == 0)
        {
            Debug.Log("Inventory empty.");
            return;
        }

        foreach (var kvp in items)
        {
            Debug.Log($"{kvp.Key} : {kvp.Value}");
        }
    }

    private void ClearInventory()
    {
        var keys = new System.Collections.Generic.List<string>(GameState.I.PlayerInventory.Items.Keys);

        foreach (var key in keys)
        {
            int count = GameState.I.PlayerInventory.GetCount(key);
            GameState.I.PlayerInventory.TryRemove(key, count);
        }
    }
}