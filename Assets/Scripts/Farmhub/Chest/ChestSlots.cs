using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ChestSlots : MonoBehaviour, IPointerClickHandler{
    public bool isChest;
    public ItemDefinition itemInSlot;
    public Sprite itemSprite;
    public bool isFull = false;
    public bool isSelected = false;
    [SerializeField] private ChestUI chestUI;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image itemImage;
    public void addItem(ItemDefinition item, int amount){
        itemInSlot = item;
        itemName.text = $"{item.DisplayName}: {amount}";
        this.itemSprite = item?.Icon;
        isFull= true;
        if(!item.IsMaterial){itemImage.sprite = itemSprite;}
    }

    public void OnPointerClick(PointerEventData eventData){
        if(GameState.I==null||itemInSlot==null){return;}
        if(eventData.button == PointerEventData.InputButton.Left){
            chestUI.DeselectAllSlots();
            this.isSelected = true;
            chestUI.TransferSelected();
            chestUI.RefreshSlots();
        }
    }

    public void clearSlot(){
        if(itemInSlot==null){return;}
        itemName.text = "ITEMNAME";
        if(!itemInSlot.IsMaterial){itemImage.sprite = null;}
        itemInSlot = null;
        isFull = false;
    }

}
