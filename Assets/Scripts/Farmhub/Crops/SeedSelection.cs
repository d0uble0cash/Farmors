using UnityEngine;

public class SeedSelection : MonoBehaviour
{
    public static SeedSelection I { get; private set; }

    [SerializeField] private ItemDefinition selectedSeed;

    public ItemDefinition SelectedSeed => selectedSeed;
    public bool HasSelectedSeed => selectedSeed != null;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
    }

    public void SelectSeed(ItemDefinition seed)
    {
        selectedSeed = seed;

        if (selectedSeed != null)
        {
            Debug.Log($"Selected seed: {selectedSeed.DisplayName}");
        }
        else
        {
            Debug.Log("Seed selection cleared.");
        }
    }

    public void ClearSelection()
    {
        SelectSeed(null);
    }
}