using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Progress")]
    public int money = 0;

    // Use HashSet at runtime for fast lookups
    public HashSet<string> rescuedAnimals = new HashSet<string>();
    public HashSet<string> unlockedAbilities = new HashSet<string>();

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

    // Small helper methods (your other systems can call these)
    public void AddMoney(int amount) => money += amount;

    public void AddRescuedAnimal(string animalId) => rescuedAnimals.Add(animalId);
    public bool HasRescuedAnimal(string animalId) => rescuedAnimals.Contains(animalId);

    public void UnlockAbility(string abilityId) => unlockedAbilities.Add(abilityId);
    public bool HasAbility(string abilityId) => unlockedAbilities.Contains(abilityId);
}