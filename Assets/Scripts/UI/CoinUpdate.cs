using UnityEngine;
using TMPro;
public class CoinUpdate : MonoBehaviour{
    [SerializeField] private TMP_Text cointText;
    private void Update(){
        if(GameState.I==null){return;}
        var items = GameState.I.PlayerInventory.Items;
        foreach(var item in items){
            ItemDefinition definition = ItemDatabase.itemDatabase.GetItemById(item.Key);
            if(definition?.Id == "gold"){
                cointText.text = $"Coins: {item.Value}ϵ";
                break;
            }
        }
    }

}
