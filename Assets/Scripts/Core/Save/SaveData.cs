using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<string> rescuedAnimals = new();
    public List<string> unlockedAbilities = new();
    public List<InventoryEntry> inventory = new();

    public string lastCheckpointID = "";
    public string lastCheckpointScene = "";
    public float lastCheckpointX = 0f;
    public float lastCheckpointY = 0f;
}