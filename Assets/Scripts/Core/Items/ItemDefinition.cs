using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string displayName;
    public string DisplayName => displayName;
    [SerializeField] private string id;
    public string Id => id;
    [SerializeField] private string description;
    public string Description => description;
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;
    [SerializeField] private int maxStackSize  = 99;   
    public int MaxStackSize => maxStackSize;
    [SerializeField] private int itemValue = 0;
    public int ItemValue => itemValue;

    
    private void OnValidate(){
        if (string.IsNullOrWhiteSpace(id)){
            Debug.LogWarning("ItemDefinition ID should not be empty!", this);
        }        else if (id.Contains(" ")){
            Debug.LogWarning("ItemDefinition ID should not contain spaces!", this);
        }
        id = id.ToLowerInvariant();
    }

}