using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase I { get; private set; }
    [SerializeField] private List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();
    private Dictionary <string, ItemDefinition> itemDict = new Dictionary<string, ItemDefinition>();

    private void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
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
            if (itemDict.ContainsKey(def.Id))
            {
                Debug.LogWarning($"Duplicate item ID '{def.Id}' found in ItemDatabase. Skipping.");
                continue;
            }
            itemDict[def.Id] = def;
        }
    }

    public ItemDefinition GetItemById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }
        if (itemDict.TryGetValue(id, out var itemDef))
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