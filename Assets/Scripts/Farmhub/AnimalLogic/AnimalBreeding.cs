using UnityEngine;

public class AnimalBreeding : MonoBehaviour
{
    [Header("Breeding")]
    [SerializeField] private float breedingRadius = 2f;
    [SerializeField] private float breedingCooldown = 120f;
    [SerializeField] private int maxSpeciesPopulation = 5;
    [SerializeField] private GameObject babyPrefab;
    [SerializeField] private LayerMask animalMask;

    [Header("Spawn")]
    [SerializeField] private float babySpawnDistance = 0.75f;

    private AnimalGrowth growth;
    private float cooldownTimer;

    private void Awake()
    {
        growth = GetComponent<AnimalGrowth>();
        cooldownTimer = breedingCooldown;
    }

    private void Update()
    {
        if (growth == null || !growth.IsAdult)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer > 0f)
            return;

        if (CountSameSpecies() >= maxSpeciesPopulation)
            return;

        TryBreed();
    }

    private void TryBreed()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, breedingRadius, animalMask);

        foreach (Collider hit in hits)
        {
            AnimalGrowth otherGrowth = hit.GetComponentInParent<AnimalGrowth>();
            AnimalBreeding otherBreeding = hit.GetComponentInParent<AnimalBreeding>();

            if (otherGrowth == null || otherBreeding == null)
                continue;

            if (otherGrowth == growth)
                continue;

            if (!otherGrowth.IsAdult)
                continue;

            if (otherGrowth.SpeciesId != growth.SpeciesId)
                continue;

            if (otherBreeding.cooldownTimer > 0f)
                continue;

            // Only one parent in the pair is allowed to spawn.
            if (GetInstanceID() > otherBreeding.GetInstanceID())
                continue;

            SpawnBaby();

            cooldownTimer = breedingCooldown;
            otherBreeding.cooldownTimer = breedingCooldown;

            return;
        }
    }

    private int CountSameSpecies()
    {
        AnimalGrowth[] allAnimals = FindObjectsByType<AnimalGrowth>(FindObjectsSortMode.None);

        int count = 0;

        foreach (AnimalGrowth animal in allAnimals)
        {
            if (animal.SpeciesId == growth.SpeciesId)
                count++;
        }

        return count;
    }

    private void SpawnBaby()
    {
        if (babyPrefab == null)
        {
            Debug.LogWarning("Baby prefab not assigned.", this);
            return;
        }

        Vector3 offset = Random.insideUnitSphere;
        offset.y = 0f;

        if (offset.sqrMagnitude < 0.001f)
            offset = Vector3.forward;

        offset = offset.normalized * babySpawnDistance;

        GameObject baby = Instantiate(
            babyPrefab,
            transform.position + offset,
            transform.rotation
        );

        AnimalGrowth babyGrowth = baby.GetComponent<AnimalGrowth>();

        if (babyGrowth != null)
        {
            babyGrowth.InitializeAsBaby();
            Debug.Log($"A baby {babyGrowth.SpeciesId} was born!", this);
        }
        else
        {
            Debug.LogWarning("Spawned baby prefab has no AnimalGrowth component.", baby);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, breedingRadius);
    }
}