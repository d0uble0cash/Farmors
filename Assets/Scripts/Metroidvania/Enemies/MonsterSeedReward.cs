using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSeedReward : MonoBehaviour
{
    [Serializable]
    public class SeedDrop
    {
        public ItemDefinition seedItem;

        [Min(1)] public int minAmount = 1;
        [Min(1)] public int maxAmount = 1;

        [Range(0f, 1f)]
        public float dropChance = 1f;
    }

    [Header("Seed Rewards")]
    [SerializeField] private List<SeedDrop> seedDrops = new List<SeedDrop>();

    [Header("Debug")]

    private bool rewardGiven = false;

    private void OnEnable()
    {
        rewardGiven = false;
    }

    public void GiveRewards()
    {
        Debug.Log($"{name}: GiveRewards was called.");

        if (rewardGiven)
        {
            Debug.LogWarning($"{name}: reward already given, stopping.");
            return;
        }

        if (GameState.I == null)
        {
            Debug.LogError($"{name}: GameState.I is null. Cannot add seeds.");
            return;
        }

        InventoryModel inventory = GameState.I.PlayerInventory;

        if (inventory == null)
        {
            Debug.LogError($"{name}: PlayerInventory is null. Cannot add seeds.");
            return;
        }

        Debug.Log($"{name}: Seed drop count = {seedDrops.Count}");

        bool gaveAnyReward = false;

        foreach (SeedDrop drop in seedDrops)
        {
            if (drop == null)
            {
                Debug.LogWarning($"{name}: drop was null.");
                continue;
            }

            if (drop.seedItem == null)
            {
                Debug.LogWarning($"{name}: seedItem is not assigned.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(drop.seedItem.Id))
            {
                Debug.LogWarning($"{name}: seedItem has empty Id.");
                continue;
            }

            float roll = UnityEngine.Random.value;

            Debug.Log($"{name}: rolling for {drop.seedItem.Id}. Roll={roll}, Chance={drop.dropChance}");

            if (roll > drop.dropChance)
            {
                Debug.Log($"{name}: failed chance roll for {drop.seedItem.Id}.");
                continue;
            }

            int min = Mathf.Min(drop.minAmount, drop.maxAmount);
            int max = Mathf.Max(drop.minAmount, drop.maxAmount);
            int amount = UnityEngine.Random.Range(min, max + 1);

            bool added = inventory.Add(drop.seedItem.Id, amount);

            Debug.Log(
                $"{name}: Add result={added}, item={drop.seedItem.Id}, amount={amount}, total now={inventory.GetCount(drop.seedItem.Id)}"
            );

            if (added)
                gaveAnyReward = true;
        }

        rewardGiven = true;

        if (gaveAnyReward)
        {
            Debug.Log($"{name}: at least one seed was added.");

            if (SaveSystem.I != null)
            {
                SaveSystem.I.Save();
                Debug.Log($"{name}: saved after seed reward.");
            }
            else
            {
                Debug.LogWarning($"{name}: SaveSystem.I is null, could not save after reward.");
            }
        }
        else
        {
            Debug.LogWarning($"{name}: no seed rewards were added.");
        }
    }
}