using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSeedReward : MonoBehaviour
{
    [Serializable]
    public class SeedDrop
    {
        [Tooltip("Must match the ItemDefinition.Id exactly. Example: seed_corn")]
        public string seedId = "seed_corn";

        [Min(1)] public int minAmount = 1;
        [Min(1)] public int maxAmount = 1;

        [Range(0f, 1f)]
        public float dropChance = 1f;
    }

    [Header("Seed Rewards")]
    [SerializeField] private List<SeedDrop> seedDrops = new List<SeedDrop>();

    [Header("Optional Player Kill Check")]
    [SerializeField] private bool requirePlayerKiller = false;
    [SerializeField] private string playerTag = "Player";

    [Header("Debug")]
    [SerializeField] private bool logRewards = true;

    private bool rewardGiven = false;

    private void OnEnable()
    {
        rewardGiven = false;
    }

    public void GiveRewards()
    {
        GiveRewardsInternal(null);
    }

    public void GiveRewardsFromKiller(GameObject killer)
    {
        GiveRewardsInternal(killer);
    }

    private void GiveRewardsInternal(GameObject killer)
    {
        if (rewardGiven)
            return;

        if (requirePlayerKiller)
        {
            if (killer == null || !killer.CompareTag(playerTag))
                return;
        }

        if (GameState.I == null)
        {
            Debug.LogWarning($"{name} could not give seed reward because GameState.I is null.");
            return;
        }

        InventoryModel inventory = GameState.I.PlayerInventory;

        if (inventory == null)
        {
            Debug.LogWarning($"{name} could not give seed reward because PlayerInventory is null.");
            return;
        }

        rewardGiven = true;

        foreach (SeedDrop drop in seedDrops)
        {
            if (drop == null)
                continue;

            if (string.IsNullOrWhiteSpace(drop.seedId))
            {
                Debug.LogWarning($"{name} has a seed drop with an empty seedId.");
                continue;
            }

            if (UnityEngine.Random.value > drop.dropChance)
                continue;

            int min = Mathf.Min(drop.minAmount, drop.maxAmount);
            int max = Mathf.Max(drop.minAmount, drop.maxAmount);
            int amount = UnityEngine.Random.Range(min, max + 1);

            bool added = inventory.Add(drop.seedId, amount);

            if (logRewards)
            {
                if (added)
                    Debug.Log($"{name} gave player {amount}x {drop.seedId}.");
                else
                    Debug.LogWarning($"{name} failed to add {amount}x {drop.seedId} to inventory.");
            }
        }
    }
}