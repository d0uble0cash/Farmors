using UnityEngine;
using TMPro;

public class VendorUI : MonoBehaviour{
    [Header("ItemTexts")]
    [SerializeField] private TMP_Text afterPurchaseText;
    public ItemDefinition buyitem;
    private ItemDefinition golditem;


    public void select(ItemDefinition buyitem){
        this.buyitem = buyitem;
    }
    public void buySelected(){
        if(GameState.I == null){return;}
        var items = GameState.I.PlayerInventory.Items;
        foreach(var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            if(definition?.DisplayName == "Gold"){
                golditem = ItemDatabase.itemDatabase.GetItemById(item.Key);
                break;
            }
        }
        if (golditem == null ||buyitem == null){afterPurchaseText.text = "Item not selected!";}
        else if (GameState.I.PlayerInventory.GetCount("gold") >= buyitem.ItemValue){
            GameState.I.PlayerInventory.TryRemove("gold", buyitem.ItemValue);
            GameState.I.PlayerInventory.Add(buyitem.Id, 1);
            afterPurchaseText.text = $"Purchased {buyitem?.DisplayName}";
        }
        else{afterPurchaseText.text = "Not enough gold!";}
    }
}