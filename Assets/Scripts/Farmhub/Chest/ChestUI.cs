using UnityEngine;
public class ChestUI : MonoBehaviour{/*
    public ChestSlot[] inventorySlots; 
    public ChestSlot[] chestSlots;


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
        items = GameState.I.ChestInventory.Items;
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

    private InventorySlot findSelected(){
        InventorySlot selectedSlot = this.selectSort(weaponitemSlots);
        if (selectedSlot==null){
            selectedSlot = this.selectSort(materialitemTexts);
        }
        return selectedSlot;
    }

    private InventorySlot selectSort(InventorySlot[] sentSlot){
        for(int i = 0; i < sentSlot.Length; i++){
            if (sentSlot[i].isSelected){return sentSlot[i];}
        }
        return null;
    }*/
}
