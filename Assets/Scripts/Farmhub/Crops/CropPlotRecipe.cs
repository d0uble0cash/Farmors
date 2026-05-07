using UnityEngine;

[System.Serializable]
public class CropRecipe
{
    public ItemDefinition seedItem;
    public ItemDefinition harvestItem;
    public int harvestAmount = 1;
    public float growTimeSeconds = 5f;
    public GameObject[] growthStages;
}