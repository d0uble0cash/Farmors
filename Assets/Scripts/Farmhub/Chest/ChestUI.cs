using UnityEngine;
public class ChestUI : MonoBehaviour{
    public ChestSlots[] inventorySlots; 
    public ChestSlots[] chestSlots;


    public void RefreshSlots(){
        if (GameState.I==null){return;}
        for(int i=0;i<inventorySlots.Length;i++){chestSlots[i].clearSlot();}
        for(int i=0;i<chestSlots.Length;i++){chestSlots[i].clearSlot();}
        var items = GameState.I.PlayerInventory.Items;
        if(items.Count==0){return;}
        foreach (var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            for(int i = 0; i < inventorySlots.Length; i++){
                if(inventorySlots[i].isFull == false){
                    inventorySlots[i].addItem(definition);
                    break;
                }
            }
        }
        items = GameState.I.PlayerInventory.Items;
        foreach(var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            for(int i = 0; i < inventorySlots.Length; i++){
                if(chestSlots[i].isFull == false){
                    chestSlots[i].addItem(definition);
                    break;
                }
            }
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
