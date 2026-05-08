using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Progress")]
    public HashSet<string> rescuedAnimals = new HashSet<string>();
    public Dictionary<string, int> rescuedAnimalCounts = new Dictionary<string, int>();
    public HashSet<string> unlockedAbilities = new HashSet<string>();

    public InventoryModel PlayerInventory { get; } = new InventoryModel();

    public CropFieldSaveData LoadedCropField { get; private set; }

    [Header("Checkpoint")]
    public string lastCheckpointID = "";
    public string lastCheckpointScene = "";
    public float lastCheckpointX = 0f;
    public float lastCheckpointY = 0f;

    [Header("References")]
    [SerializeField] private PlacementController placementController;

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

    public void SetPlacementController(PlacementController controller)
    {
        placementController = controller;
    }

    public void InitializeNewGame()
    {
        rescuedAnimals.Clear();
        rescuedAnimalCounts.Clear();
        unlockedAbilities.Clear();
        PlayerInventory.Clear();

        LoadedCropField = new CropFieldSaveData();

        PlayerInventory.Add("seed_corn", 10);
    }

    public void AddRescuedAnimal(string animalId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(animalId))
            return;

        rescuedAnimals.Add(animalId);

        if (!rescuedAnimalCounts.ContainsKey(animalId))
            rescuedAnimalCounts[animalId] = 0;

        rescuedAnimalCounts[animalId] += amount;
    }

    public bool HasRescuedAnimal(string animalId)
    {
        return rescuedAnimals.Contains(animalId);
    }

    public int GetRescuedAnimalCount(string animalId)
    {
        return rescuedAnimalCounts.TryGetValue(animalId, out int count) ? count : 0;
    }

    public void UnlockAbility(string abilityId)
    {
        unlockedAbilities.Add(abilityId);
    }

    public bool HasAbility(string abilityId)
    {
        return unlockedAbilities.Contains(abilityId);
    }

    public SaveData ToSaveData()
    {
        List<AnimalCountSaveData> animalCounts = new List<AnimalCountSaveData>();

        foreach (var pair in rescuedAnimalCounts)
        {
            animalCounts.Add(new AnimalCountSaveData
            {
                animalId = pair.Key,
                count = pair.Value
            });
        }

        CropFieldSaveData cropField = placementController != null
            ? placementController.GetSaveData()
            : LoadedCropField ?? new CropFieldSaveData();

        return new SaveData
        {
            rescuedAnimals = new List<string>(rescuedAnimals),
            rescuedAnimalCounts = animalCounts,
            unlockedAbilities = new List<string>(unlockedAbilities),
            inventory = PlayerInventory.ToSnapshot(),

            cropField = cropField,

            lastCheckpointID = lastCheckpointID,
            lastCheckpointScene = lastCheckpointScene,
            lastCheckpointX = lastCheckpointX,
            lastCheckpointY = lastCheckpointY
        };
    }

    public void LoadFromSaveData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError("Failed to load save data.");
            return;
        }

        PlayerInventory.LoadFromSnapshot(data.inventory);

        rescuedAnimals.Clear();
        if (data.rescuedAnimals != null)
        {
            foreach (var id in data.rescuedAnimals)
                rescuedAnimals.Add(id);
        }

        rescuedAnimalCounts.Clear();
        if (data.rescuedAnimalCounts != null)
        {
            foreach (var entry in data.rescuedAnimalCounts)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.animalId))
                    continue;

                rescuedAnimalCounts[entry.animalId] = Mathf.Max(0, entry.count);
                rescuedAnimals.Add(entry.animalId);
            }
        }

        unlockedAbilities.Clear();
        if (data.unlockedAbilities != null)
        {
            foreach (var id in data.unlockedAbilities)
                unlockedAbilities.Add(id);
        }

        LoadedCropField = data.cropField ?? new CropFieldSaveData();

        lastCheckpointID = data.lastCheckpointID;
        lastCheckpointScene = data.lastCheckpointScene;
        lastCheckpointX = data.lastCheckpointX;
        lastCheckpointY = data.lastCheckpointY;
    }
}