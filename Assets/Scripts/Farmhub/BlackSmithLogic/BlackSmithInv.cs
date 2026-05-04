using TMPro;
using UnityEngine;
public class BlackSmithInv : MonoBehaviour{
    public BlackSmithInvSlot[] weaponitemSlots;
    public void UpdateBlackSmith(){
        RefreshSlots();
    }

    private void RefreshSlots(){
        if (GameState.I==null){return;}
        this.ClearSlot(weaponitemSlots);
        var items = GameState.I.PlayerInventory.Items;
        if(items.Count==0){return;}
        foreach (var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            if(!definition.IsMaterial){
                switch(definition?.Id ?? item.Key){
                    case "sword":
                    case "bow":
                    case "greatsword":
                    case "darksword":
                    case "crossbow":
                        for(int i = 0; i < weaponitemSlots.Length; i++){
                            if (weaponitemSlots[i].isFull == false){
                                weaponitemSlots[i].addItem(definition);
                                break;
                            }
                        }
                        break;
                    
                }
            }
        }
    }

    public void Upgrade(){
        if (GameState.I==null){return;}
        BlackSmithInvSlot selectedSlot = selectSort(weaponitemSlots);
        if(selectedSlot==null){return;}
        ItemDefinition definition = selectedSlot.itemInSlot;
        if(definition==null){return;}
        if (GameState.I.PlayerInventory.GetCount("gold")>= selectedSlot.itemResourcesCost && GameState.I.PlayerInventory.GetCount("wood")>=selectedSlot.itemResourceWood
        && GameState.I.PlayerInventory.GetCount("string")>= selectedSlot.itemResourceString && GameState.I.PlayerInventory.GetCount("steel")>= selectedSlot.itemResourceSteel){
            GameState.I.PlayerInventory.TryRemove("gold", selectedSlot.itemResourcesCost);
            GameState.I.PlayerInventory.TryRemove("wood", selectedSlot.itemResourceWood);
            GameState.I.PlayerInventory.TryRemove("string", selectedSlot.itemResourceString);
            GameState.I.PlayerInventory.TryRemove("steel", selectedSlot.itemResourceSteel);
            definition.ChangeDamage(selectedSlot.itemUpgradeDamage);
            definition.ChangeRange(selectedSlot.itemUpgradeRange);
        }
        this.RefreshSlots();
    }

    public void DeselectAllSlots(){
        for(int i=0;i<weaponitemSlots.Length; i++){weaponitemSlots[i].isSelected = false;}
    }

    private BlackSmithInvSlot selectSort(BlackSmithInvSlot[] sentSlot){
        for(int i = 0; i < sentSlot.Length; i++){
            if (sentSlot[i].isSelected){return sentSlot[i];}
        }
        return null;
    }
    private void ClearSlot(BlackSmithInvSlot[] sentSlots){
        for(int i=0;i<sentSlots.Length;i++){sentSlots[i].clearSlot();}
    }

}
