using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour{
    [Header("Refs")]
    [SerializeField] private TMP_Text inventoryText;
    public GameObject InventoryMenu;
    public InventorySlot[] weaponitemSlots, farmitemSlots;
    private void Awake(){
        if (inventoryText == null)
            inventoryText = GetComponentInChildren<TMP_Text>();
    }

    private void Update(){
        RefreshSlots();
        RefreshText();
    }
    private void RefreshText(){
        if (inventoryText == null) {return;}

        if (GameState.I == null){
            inventoryText.text = "Inventory:\nUnavailable";
            return;
        }
        
        string newText = "Inventory:\n";
        var items = GameState.I.PlayerInventory.Items;

        if (items.Count == 0){newText += "Empty";}
        else{
            foreach (var item in items){
                ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
                if (definition.IsMaterial){
                    newText += $"{definition?.DisplayName ?? item.Key}: {item.Value}\n";
                }
            }
        }
        inventoryText.text = newText;
    }

    private void RefreshSlots(){
        if (GameState.I==null){return;}
        var items = GameState.I.PlayerInventory.Items;
        if(items.Count==0){return;}
        foreach (var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            if(!definition.IsMaterial){
                switch(definition?.Id ?? item.Key){
                    case "shears":
                    case "pitchfork":
                        for(int i = 0; i < farmitemSlots.Length; i++){
                            if (farmitemSlots[i].isFull == false){
                                farmitemSlots[i].addItem(definition?.DisplayName ?? item.Key, 
                                definition?.Icon, 
                                definition?.Description ?? item.Key, 
                                definition.ItemValue);
                                break;
                            }
                        }
                        break;
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