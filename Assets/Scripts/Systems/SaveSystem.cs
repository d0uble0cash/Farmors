using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem I { get; private set; }
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

        var data = GameState.I.ToSaveData();
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

        if (data == null)
        {
            Debug.LogError("Failed to load save data. Starting fresh.");
            return;
        }

        GameState.I.LoadFromSaveData(data);
        Debug.Log($"Loaded from: {SavePath}");
    }

    public bool HasSave() => File.Exists(SavePath);

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log($"Deleted save file: {SavePath}");
        }
        else
        {
            Debug.Log("No save file to delete.");
        }

        if (GameState.I != null)
        {
            GameState.I.InitializeNewGame();
        }
    }
}