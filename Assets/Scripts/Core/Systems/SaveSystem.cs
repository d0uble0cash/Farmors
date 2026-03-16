using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem I { get; private set; }

    [Serializable]
    public class SaveData
    {
        public int money;
        public List<string> rescuedAnimals = new();
        public List<string> unlockedAbilities = new();
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

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

    public void Save()
    {
        if (GameState.I == null)
        {
            Debug.LogWarning("No GameState found. Cannot save.");
            return;
        }

        var data = new SaveData
        {
            money = GameState.I.money,
            rescuedAnimals = new List<string>(GameState.I.rescuedAnimals),
            unlockedAbilities = new List<string>(GameState.I.unlockedAbilities)
        };

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Saved to: {SavePath}");
    }

    public void Load()
    {
        if (GameState.I == null)
        {
            Debug.LogWarning("No GameState found. Cannot load.");
            return;
        }

        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found. Starting fresh.");
            return;
        }
        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<SaveData>(json);

        GameState.I.money = data.money;

        GameState.I.rescuedAnimals.Clear();
        foreach (var id in data.rescuedAnimals)
            GameState.I.rescuedAnimals.Add(id);

        GameState.I.unlockedAbilities.Clear();
        foreach (var id in data.unlockedAbilities)
            GameState.I.unlockedAbilities.Add(id);

        Debug.Log($"Loaded from: {SavePath}");
    }
}