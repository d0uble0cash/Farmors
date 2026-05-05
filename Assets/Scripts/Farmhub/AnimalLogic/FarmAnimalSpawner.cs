using System.Collections.Generic;
using UnityEngine;

public class FarmAnimalSpawner : MonoBehaviour
{
    [System.Serializable]
    public class AnimalSpawnEntry
    {
        public string animalId;
        public GameObject prefab;
        public Transform[] spawnPoints;
    }

    [Header("Animal Spawn Entries")]
    [SerializeField] private AnimalSpawnEntry[] animals;

    private readonly List<GameObject> spawnedAnimals = new();

    private void Start()
    {
        SpawnSavedAnimals();
    }

    public void SpawnSavedAnimals()
    {
        ClearSpawnedAnimals();

        if (GameState.I == null)
        {
            Debug.LogWarning("GameState.I is null. Cannot spawn saved animals.", this);
            return;
        }

        foreach (AnimalSpawnEntry entry in animals)
        {
            if (entry == null || entry.prefab == null)
                continue;

            int count = GameState.I.GetRescuedAnimalCount(entry.animalId);

            if (count <= 0)
                continue;

            if (entry.spawnPoints == null || entry.spawnPoints.Length == 0)
            {
                Debug.LogWarning($"No spawn points set for animal: {entry.animalId}", this);
                continue;
            }

            for (int i = 0; i < count; i++)
            {
                Transform spawnPoint = entry.spawnPoints[i % entry.spawnPoints.Length];

                GameObject animal = Instantiate(
                    entry.prefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

                spawnedAnimals.Add(animal);
            }
        }
    }

    private void ClearSpawnedAnimals()
    {
        foreach (GameObject animal in spawnedAnimals)
        {
            if (animal != null)
                Destroy(animal);
        }

        spawnedAnimals.Clear();
    }
}