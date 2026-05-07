using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SeedBarSlot : MonoBehaviour{
    public ItemDefinition itemInSlot;
    public bool isFull = false;
    public bool isSelected = false;
    [SerializeField] private SeedSelectionCheat seedUI;
    [SerializeField] private TMP_Text itemName;

    public void AddItem(ItemDefinition item){
        itemInSlot = item;
        itemName.text = item.DisplayName;
        isFull= true;
    }

    public void OnPointerClick(PointerEventData eventData){
        if(GameState.I==null||itemInSlot==null){return;}
        if(eventData.button == PointerEventData.InputButton.Left){
            seedUI.RefreshHotBar();
            this.isSelected = true;
        }
    }

    public void ClearSlot(){
        if(itemInSlot==null){return;}
        itemName.text = "SLOT1";
        itemInSlot = null;
        isFull = false;
    }

}