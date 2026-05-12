using System.Collections.Generic;
using UnityEngine;

public class ChestInventory : MonoBehaviour
{
    [SerializeField] private string chestId;
    [SerializeField] private List<InventoryEntry> startingItems;

    public string ChestId => chestId;
    public InventoryModel Inventory { get; private set; } = new InventoryModel();

    private void Awake(){
        Inventory.LoadFromSnapshot(startingItems);
    }

    public List<InventoryEntry> ToSnapshot(){
        return Inventory.ToSnapshot();
    }

    public void LoadFromSnapshot(List<InventoryEntry> savedItems){
        Inventory.LoadFromSnapshot(savedItems);
    }
}