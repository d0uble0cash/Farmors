using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InventorySlot : MonoBehaviour, IPointerClickHandler{
    
    //ITEMDATA//
    public string itemName;
    public Sprite itemSprite;
    public bool isFull = false;
    public bool isSelected = false;
    public string itemDescription;
    public int itemCost;
    public Sprite emptySprite;
    //ITEMDESCRIPTIONslot
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    //public TMP_Text itemDescriptionCost;
    [SerializeField] private InventoryUI inventoryUI;
    public void SetInventoryUI(InventoryUI ui){
        inventoryUI = ui;
    }
    //INVENTORYSLOT//
    [SerializeField] private Image itemImage;
    public void addItem(string itemName, Sprite itemSprite, string itemDescription, int itemCost){
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        this.itemCost = itemCost;
        isFull= true;

        itemImage.sprite = itemSprite;
    }

    public void OnPointerClick(PointerEventData eventData){
        if(eventData.button == PointerEventData.InputButton.Left && itemSprite!=null){
            inventoryUI.DeselectAllSlots();
            this.isSelected = true;
            itemDescriptionNameText.text = itemName;
            itemDescriptionText.text = itemDescription;
            //itemDescriptionCost.text = itemCost.ToString() + "ϵ";
            //if(itemDescriptionCost.text == null){
            //    itemDescriptionCost.text = "";
            //}
        }
    }
}