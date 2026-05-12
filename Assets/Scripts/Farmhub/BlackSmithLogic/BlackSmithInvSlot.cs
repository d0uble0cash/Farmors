using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BlackSmithInvSlot : MonoBehaviour, IPointerClickHandler{
    //ITEMDATA//
    public ItemDefinition itemInSlot;
    public string itemName;
    public Sprite itemSprite;
    public bool isFull = false;
    public bool isSelected = false;
    public int itemResourcesCost;
    public int itemResourceWood;
    public int itemResourceSteel;
    public int itemResourceString;
    public int itemUpgradeDamage;
    public int itemUpgradeRange;
    //ITEMDESCRIPTIONslot
    public TMP_Text itemNameText;
    public TMP_Text itemStatsText;
    public TMP_Text itemResourcesText;
    public TMP_Text itemUpgradedText;
    public TMP_Text itemCostText;
    [SerializeField] private BlackSmithInv blackSmithInv;
    //INVENTORYSLOT//
    [SerializeField] private Image itemImage;

    public void addItem(ItemDefinition item){
        itemInSlot = item;
        itemName = item?.DisplayName;
        itemResourcesCost = item.ItemValue*(int)Math.Pow(item.ItemValue, 1/3);
        itemSprite = item.Icon;
        itemUpgradeDamage = itemInSlot.Damage+(int)Math.Pow(itemInSlot.Damage, 1/3);
        itemUpgradeRange = itemInSlot.Range+(int)Math.Pow(itemInSlot.Range, 1/3);
        itemResourceWood = (int)(itemInSlot.ItemValue*1.2);
        itemResourceString = itemInSlot.Range*2;
        if(itemResourceString==0){itemResourceSteel = (int)(itemInSlot.Damage*1.5);}
        isFull= true;
        itemImage.sprite = itemSprite;
    }

    public void OnPointerClick(PointerEventData eventData){
        if(GameState.I==null||itemInSlot==null){return;}
        if(eventData.button == PointerEventData.InputButton.Left){
            blackSmithInv.DeselectAllSlots();
            this.isSelected = true;
            if (!itemInSlot.IsMaterial){
                itemNameText.text = itemName;
                
                if (itemInSlot.Id=="bow"||itemInSlot.Id=="crossbow"){
                    itemStatsText.text = $"Damage: {itemInSlot.Damage}\nRange: {itemInSlot.Range}";
                    itemResourcesText.text = $"Resources:\nWood: {itemResourceWood}\nString: {itemResourceString}";
                    itemUpgradedText.text = $"Damage: {itemUpgradeDamage}\nRange: {itemUpgradeRange}";
                }
                else{
                    itemStatsText.text = $"Damage: {itemInSlot.Damage}";
                    itemResourcesText.text = $"Resources:\nWood: {itemResourceWood}\nSteel: {itemResourceSteel}";
                    itemUpgradedText.text = $"Damage: {itemUpgradeDamage}";
                }
                
                itemCostText.text = $"Cost: {itemResourcesCost}ϵ";
            }
        }
    }

    public void clearSlot(){
        if(itemInSlot==null){return;}
        itemNameText.text = null;
        itemStatsText.text = null;
        itemResourcesText.text = null;
        itemUpgradedText.text = null;
        itemCostText.text = null;
        itemInSlot = null;
        isFull = false;
    }
}
