using UnityEngine;
public class ChestUI : MonoBehaviour{

    private InventoryModel playerInventory;
    private InventoryModel chestInventory;
    public ChestSlots[] inventorySlots; 
    public ChestSlots[] chestSlots;

    public void Show(InventoryModel inventory, InventoryModel targetInventory = null){
        chestInventory = inventory;
        playerInventory = targetInventory;
        gameObject.SetActive(true);
        this.RefreshSlots();
    }


    public void RefreshSlots(){
        if (GameState.I==null){return;}
        for(int i=0;i<inventorySlots.Length;i++){inventorySlots[i].clearSlot();}
        for(int i=0;i<chestSlots.Length;i++){chestSlots[i].clearSlot();}
        var items = playerInventory.Items;
        if(items.Count==0){return;}
        foreach (var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            if(definition.Id!="gold"){
                for(int i = 0; i < inventorySlots.Length; i++){
                    if(inventorySlots[i].isFull == false){
                        inventorySlots[i].addItem(definition, item.Value);
                        break;
                    }
                }
            }
        }
        items = chestInventory.Items;
        foreach(var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            if(definition.Id!="gold"){
                for(int i = 0; i < inventorySlots.Length; i++){
                    if(chestSlots[i].isFull == false){
                        chestSlots[i].addItem(definition, item.Value);
                        break;
                    }
                }
            }
        }
    }

    public void TransferSelected(){
        ChestSlots selected = findSelected();

        if (selected == null)
            return;

        ItemDefinition definition = selected.itemInSlot;

        if (definition == null){
            Debug.Log("Definition is null");
            return;
        }
        InventoryModel current;
        InventoryModel other;
        if (selected.isChest){
            current = chestInventory;
            other = playerInventory;
        }
        else{
            current = playerInventory;
            other = chestInventory;
        }
        if (current == null || other == null){
            Debug.Log("Either is null");
            return;
        }
        bool transferred = InventoryTransfer.Transfer(current, other, definition.Id, 1);

        if (transferred){
            DeselectAllSlots();
            RefreshSlots();
        }
    }
    public void ChestClicked(){
        if(GameState.I==null){return;}

    }
    public void DeselectAllSlots(){
        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i].isSelected = false;

        for (int i = 0; i < chestSlots.Length; i++)
            chestSlots[i].isSelected = false;

    }
    private ChestSlots findSelected(){
        ChestSlots selectedSlot = selectSort(inventorySlots);
        if (selectedSlot == null)
            selectedSlot = selectSort(chestSlots);

        return selectedSlot;
    }

    private ChestSlots selectSort(ChestSlots[] sentSlot){
        for (int i = 0; i < sentSlot.Length; i++){
            if (sentSlot[i].isSelected)
                return sentSlot[i];
        }
        return null;
    }
}
