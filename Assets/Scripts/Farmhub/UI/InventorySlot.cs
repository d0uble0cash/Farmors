using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InventorySlot : MonoBehaviour, IPointerClickHandler{
    
    //ITEMDATA//
    public ItemDefinition itemInSlot;
    public string itemName;
    public Sprite itemSprite;
    public bool isFull = false;
    public bool isSelected = false;
    public string itemDescription;
    public int itemCost;
    //ITEMDESCRIPTIONslot
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemDescriptionCost;
    [SerializeField] private InventoryUI inventoryUI;
    //INVENTORYSLOT//
    [SerializeField] private Image itemImage;
    //private void Start(){
        //inventoryUI = GameObject.Find("StartInv").GetComponent<InventoryUI>();
    //}
    public void addItem(ItemDefinition item){
        itemInSlot = item;
        this.itemName = item?.DisplayName;
        this.itemCost = item.ItemValue;
        this.itemSprite = item?.Icon;
        this.itemDescription = item?.Description;
        isFull= true;

        if(!item.IsMaterial){itemImage.sprite = itemSprite;}
    }

    public void addItemM(ItemDefinition item, int itemValue){
        itemInSlot = item;
        this.itemName = item?.DisplayName;
        this.itemCost = item.ItemValue;
        itemDescriptionNameText.text = itemName;
        itemDescriptionCost.text = $": {itemValue}";
        isFull = true;
    }

    public void OnPointerClick(PointerEventData eventData){
        if(GameState.I==null||itemInSlot==null){return;}
        if(eventData.button == PointerEventData.InputButton.Left){
            inventoryUI.DeselectAllSlots();
            this.isSelected = true;
            if (!itemInSlot.IsMaterial){
                itemDescriptionNameText.text = itemName;
                itemDescriptionText.text = $"Damage: {itemInSlot.Damage}";
                itemDescriptionCost.text = $"Cost: {itemCost}ϵ";
            }
        }
    }
    public void clearSlot(){
        if(itemInSlot==null){return;}
        itemDescriptionNameText.text = "ITEMNAME";
        itemDescriptionText.text = "Description: ";
        itemDescriptionCost.text = "Cost: ";
        if(!itemInSlot.IsMaterial){itemImage.sprite = null;}
        itemInSlot = null;
        isFull = false;
    }
}