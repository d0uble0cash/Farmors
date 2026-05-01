using UnityEngine;
using TMPro;

public class VendorUI : MonoBehaviour{
    [Header("ItemTexts")]
    [SerializeField] private TMP_Text afterPurchaseText;
    private ItemDefinition selected;
    private ItemDefinition buyitem;
    private ItemDefinition golditem;


    public void select(ItemDefinition selected){
        this.selected = selected;
    }
    public void buySelected(){
        var items = GameState.I.PlayerInventory.Items;
        foreach(var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            if(definition?.DisplayName == "Gold"){
                golditem = ItemDatabase.itemDatabase.GetItemById(item.Key);
                break;
            }
        }
        /*switch (selected){
            case 0: buyitem = ; break;
            case 1: buyitem = ; break;
            case 2: buyitem = ; break;
            case 3: buyitem = ; break;
            case 4: buyitem = ; break;
            case 5: buyitem = ; break;
            case 6: buyitem = ; break;
            default: afterPurchaseText.text = "Item not selected!"; return;
        }*/
        if (golditem.ItemValue >= buyitem.ItemValue){
            //golditem.ItemValue = golditem.ItemValue - buyitem.ItemValue;
            GameState.I.PlayerInventory.Add(buyitem.Id, 1);
            //afterPurchaseText.text = $"Purchased {buyitem?.DisplayName ?? buyitem.Key}";
        }
        else{afterPurchaseText.text = "Not enough gold!";}
        



    }




}