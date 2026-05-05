using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour{
    [Header("Refs")]
    //[SerializeField] private TMP_Text inventoryText;
    //public GameObject InventoryMenu;
    public InventorySlot[] weaponitemSlots, farmitemSlots, materialitemTexts;
    [SerializeField] public GameObject sellCheckPanelMaterial; 
    [SerializeField] private GameObject sellCheckPanel;
    [SerializeField] private TMP_InputField matInput;
    [SerializeField] private TMP_Text MaterialText;
    [SerializeField] private TMP_Text SellText;

    public void UpdateUI(){
        RefreshSlots();
    }

    private void RefreshSlots(){
        if (GameState.I==null){return;}
        this.ClearSlot(weaponitemSlots); this.ClearSlot(materialitemTexts); this.ClearSlot(farmitemSlots);
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
                                farmitemSlots[i].addItem(definition);
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
                                weaponitemSlots[i].addItem(definition);
                                break;
                            }
                        }
                        break;
                }
            }
            else if(definition?.Id != "gold"){
                for(int i = 0; i < materialitemTexts.Length; i++){
                    if(materialitemTexts[i].isFull == false){
                        materialitemTexts[i].addItemM(definition, item.Value);
                        break;
                    }
                }
            }
        }
    }

    public void SellSelectFirst(){
        ItemDefinition definition = findSelected().itemInSlot;
        if(definition==null||definition.Id=="gold"){return;}
        else if(definition.IsMaterial){
            sellCheckPanelMaterial.SetActive(true);
            MaterialText.text = $"Sell {definition.DisplayName}?";
        }
        else{
            sellCheckPanel.SetActive(true);
            SellText.text = $"Sell {definition.DisplayName}?";
        }
    }

    public void SellSelected(){
        ItemDefinition definition = findSelected().itemInSlot;
        if(definition==null){return;}
        GameState.I.PlayerInventory.Add("gold", definition.ItemValue);
        GameState.I.PlayerInventory.TryRemove(definition.Id, 1);
        this.UpdateUI();
    }

    public void SellSelectedM(){
        int sellAmount = int.Parse(matInput.text);
        ItemDefinition definition = findSelected().itemInSlot;
        if(definition==null||GameState.I.PlayerInventory.GetCount(definition.Id)<sellAmount){return;}
        GameState.I.PlayerInventory.Add("gold", definition.ItemValue*sellAmount);
        GameState.I.PlayerInventory.TryRemove(definition.Id, sellAmount);
        this.UpdateUI();
    }

    public void DeselectAllSlots(){
        for(int i=0;i<weaponitemSlots.Length; i++){weaponitemSlots[i].isSelected = false;}
        for(int i=0;i<farmitemSlots.Length; i++){farmitemSlots[i].isSelected = false;}
        for (int i=0;i<materialitemTexts.Length;i++){materialitemTexts[i].isSelected = false;}
    }

    private void ClearSlot(InventorySlot[] sentSlots){
        for(int i=0;i<sentSlots.Length;i++){sentSlots[i].clearSlot();}
    }

    private InventorySlot findSelected(){
        InventorySlot selectedSlot = this.selectSort(weaponitemSlots);
        if (selectedSlot==null){
            selectedSlot = this.selectSort(materialitemTexts);
            if (selectedSlot == null){
                selectedSlot = this.selectSort(farmitemSlots);
            }
        }
        return selectedSlot;
    }

    private InventorySlot selectSort(InventorySlot[] sentSlot){
        for(int i = 0; i < sentSlot.Length; i++){
            if (sentSlot[i].isSelected){return sentSlot[i];}
        }
        return null;
    }
}