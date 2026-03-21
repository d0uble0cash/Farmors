using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Progress")]
    // Use HashSet at runtime for fast lookups
    public HashSet<string> rescuedAnimals = new HashSet<string>();
    public HashSet<string> unlockedAbilities = new HashSet<string>();
    public InventoryModel PlayerInventory { get; } = new InventoryModel();

    public void AddRescuedAnimal(string animalId) => rescuedAnimals.Add(animalId);
    public bool HasRescuedAnimal(string animalId) => rescuedAnimals.Contains(animalId);
    public void UnlockAbility(string abilityId) => unlockedAbilities.Add(abilityId);
    public bool HasAbility(string abilityId) => unlockedAbilities.Contains(abilityId);

    private void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializeNewGame()
    {
        rescuedAnimals.Clear();
        unlockedAbilities.Clear();
        PlayerInventory.Clear();

        PlayerInventory.Add("seed_corn", 10);
    }

    public SaveData ToSaveData()
    {
        return new SaveData
        {
            rescuedAnimals = new List<string>(rescuedAnimals),
            unlockedAbilities = new List<string>(unlockedAbilities),
            inventory = PlayerInventory.ToSnapshot()
        };
    }
    public void LoadFromSaveData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError("Failed to load save data. Starting fresh.");
            return;
        }

        PlayerInventory.LoadFromSnapshot(data.inventory);

        rescuedAnimals.Clear();
        foreach (var id in data.rescuedAnimals)
            rescuedAnimals.Add(id);

        unlockedAbilities.Clear();
        foreach (var id in data.unlockedAbilities)
            unlockedAbilities.Add(id);
    }
}