using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    [Header("Spawn Spread")]
    [SerializeField] private float randomSpawnRadius = 1.25f;
    [SerializeField] private bool sampleNavMesh = true;

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
                Vector3 spawnPosition = GetSpawnPosition(spawnPoint.position);

                GameObject animal = Instantiate(
                    entry.prefab,
                    spawnPosition,
                    spawnPoint.rotation
                );

                spawnedAnimals.Add(animal);
            }
        }
    }

    private Vector3 GetSpawnPosition(Vector3 basePosition)
    {
        Vector2 randomCircle = Random.insideUnitCircle * randomSpawnRadius;
        Vector3 candidate = basePosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

        if (!sampleNavMesh)
            return candidate;

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            return hit.position;

        return basePosition;
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