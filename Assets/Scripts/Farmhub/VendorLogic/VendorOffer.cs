using UnityEngine;

[System.Serializable]
public class VendorOffer
{
    [SerializeField] private ItemDefinition item;
    public ItemDefinition Item => item;

    [SerializeField] private int price = 1;
    public int Price => price;

    [SerializeField] private int quantity = 1;
    public int Quantity => quantity;

    public bool IsValid()
    {
        return item != null && price > 0 && quantity > 0;
    }
}