using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase itemDatabase { get; private set; }
    [SerializeField] private List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();
    [SerializeField] private Dictionary <string, ItemDefinition> itemDictionary = new Dictionary<string, ItemDefinition>();

    private void Awake()
    {
        if (itemDatabase != null)
        {
            Destroy(gameObject);
            return;
        }
        itemDatabase = this;
        DontDestroyOnLoad(gameObject);
        
        foreach (var def in itemDefinitions)
        {
            if (def == null) {
                continue;
            }
            if (string.IsNullOrWhiteSpace(def.Id))
            {
                continue;
            }
            if (itemDictionary.ContainsKey(def.Id))
            {
                Debug.LogWarning($"Duplicate item ID '{def.Id}' found in ItemDatabase. Skipping.");
                continue;
            }
            itemDictionary[def.Id] = def;
        }
    }

    public ItemDefinition GetItemById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }
        if (itemDictionary.TryGetValue(id, out var itemDef))
        {
            return itemDef;
        }
        else
        {
            Debug.LogWarning($"Item with ID '{id}' not found in database.");
            return null;
        }
    }
}