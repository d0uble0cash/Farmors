using TMPro;
using UnityEngine;
public class BlackSmithInv : MonoBehaviour{
    

    public InventorySlot[] weaponitemSlots;


    public void Update(){
        RefreshSlots();
    }

    private void RefreshSlots(){
        if (GameState.I==null){return;}
        var items = GameState.I.PlayerInventory.Items;
        if(items.Count==0){return;}
        foreach (var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            if(definition?.Icon ?? item.Key !=null){
                switch(definition?.Id ?? item.Key){
                    case "sword":
                    case "bow":
                    case "greatsword":
                    case "darksword":
                    case "crossbow":
                        for(int i = 0; i < weaponitemSlots.Length; i++){
                            if (weaponitemSlots[i].isFull == false){
                                weaponitemSlots[i].addItem(definition?.DisplayName ?? item.Key, 
                                definition?.Icon, 
                                definition?.Description ?? item.Key, 
                                definition.ItemValue);
                                break;
                            }
                        }
                        break;
                    
                }
            }
        }
    }




}
