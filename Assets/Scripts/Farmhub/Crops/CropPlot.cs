using UnityEngine;

public class CropPlot : MonoBehaviour, IInteractable
{
    public enum PlotState
    {
        Empty,
        Growing,
        Ready
    }

    [System.Serializable]
    public class CropRecipe
    {
        public ItemDefinition seedItem;
        public ItemDefinition harvestItem;
        public int harvestAmount = 1;
        public float growTimeSeconds = 5f;
    }

    [Header("Visuals")]
    [SerializeField] private Renderer plotRenderer;
    [SerializeField] private Transform cropVisual;
    [SerializeField] private Vector3 cropScaleMin = new(0.2f, 0.2f, 0.2f);
    [SerializeField] private Vector3 cropScaleMax = new(0.8f, 1.0f, 0.8f);

    [Header("World Prompt")]
    [SerializeField] private WorldPrompt prompt;

    [Header("Interaction")]
    [SerializeField] private Collider interactCollider;

    [Header("Crop Recipes")]
    [SerializeField] private CropRecipe[] cropRecipes;

    public PlotState State => state;
    public bool CanInteract => state == PlotState.Empty || state == PlotState.Ready;

    private PlotState state = PlotState.Empty;
    private CropRecipe currentCrop;
    private float growTimer = 0f;
    private float currentGrowTime = 5f;
    private bool isFocused = false;

    private MaterialPropertyBlock mpb;
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (plotRenderer == null)
            plotRenderer = GetComponentInChildren<Renderer>();

        if (interactCollider == null)
            interactCollider = GetComponentInChildren<Collider>();

        mpb = new MaterialPropertyBlock();

        SetState(PlotState.Empty, true);

        if (prompt != null)
        {
            prompt.Show(false);
            prompt.SetPulsing(false);
        }
    }

    private void Update()
    {
        TickGrowth();
    }

    private void Reset()
    {
        plotRenderer = GetComponentInChildren<Renderer>();
        interactCollider = GetComponentInChildren<Collider>();
    }

    private void TickGrowth()
    {
        if (state != PlotState.Growing)
            return;

        growTimer += Time.deltaTime;

        if (growTimer >= currentGrowTime)
        {
            growTimer = currentGrowTime;
            SetState(PlotState.Ready, false);
            return;
        }

        ApplyCropGrowthVisual();
    }

    public void Interact()
    {
        if (!CanInteract)
            return;

        if (GameState.I == null)
        {
            Debug.LogError("GameState.I is null. Start from Boot scene so Core singletons exist.", this);
            return;
        }

        if (GameState.I.PlayerInventory == null)
        {
            Debug.LogError("PlayerInventory is missing on GameState.", this);
            return;
        }

        switch (state)
        {
            case PlotState.Empty:
                PlantSelectedSeed();
                break;

            case PlotState.Ready:
                HarvestCrop();
                break;
        }
    }

    private void PlantSelectedSeed()
    {
        if (SeedSelection.I == null || SeedSelection.I.SelectedSeed == null)
        {
            Debug.Log("No seed selected.", this);
            RefreshPrompt();
            return;
        }

        ItemDefinition selectedSeed = SeedSelection.I.SelectedSeed;
        CropRecipe recipe = GetRecipeForSeed(selectedSeed);

        if (recipe == null)
        {
            Debug.Log($"This plot cannot plant {selectedSeed.DisplayName}.", this);
            RefreshPrompt();
            return;
        }

        if (!GameState.I.PlayerInventory.TryRemove(selectedSeed.Id, 1))
        {
            Debug.Log($"Not enough seeds: {selectedSeed.DisplayName}", this);
            RefreshPrompt();
            return;
        }

        currentCrop = recipe;
        currentGrowTime = Mathf.Max(0.01f, recipe.growTimeSeconds);

        SetState(PlotState.Growing, true);
    }

    private void HarvestCrop()
    {
        if (currentCrop == null || currentCrop.harvestItem == null)
        {
            Debug.LogError("No current crop set.", this);
            return;
        }

        GameState.I.PlayerInventory.Add(currentCrop.harvestItem.Id, currentCrop.harvestAmount);
        Debug.Log($"Harvested {currentCrop.harvestAmount}x {currentCrop.harvestItem.DisplayName}!", this);

        currentCrop = null;
        SetState(PlotState.Empty, true);
    }

    private CropRecipe GetRecipeForSeed(ItemDefinition seed)
    {
        if (seed == null || cropRecipes == null)
            return null;

        foreach (CropRecipe recipe in cropRecipes)
        {
            if (recipe == null || recipe.seedItem == null || recipe.harvestItem == null)
                continue;

            if (recipe.seedItem.Id == seed.Id)
                return recipe;
        }

        return null;
    }

    private string GetEmptyPromptText()
    {
        if (SeedSelection.I == null || SeedSelection.I.SelectedSeed == null)
            return "Select Seed";

        ItemDefinition selectedSeed = SeedSelection.I.SelectedSeed;
        CropRecipe recipe = GetRecipeForSeed(selectedSeed);

        if (recipe == null)
            return "Wrong Seed";

        if (GameState.I == null || GameState.I.PlayerInventory == null)
            return "No Inventory";

        if (!GameState.I.PlayerInventory.Has(selectedSeed.Id))
            return "Need Seeds";

        return $"Plant {selectedSeed.DisplayName}";
    }

    public void SetFocused(bool focused)
    {
        isFocused = focused;
        RefreshPrompt();
    }

    private void RefreshPrompt()
    {
        if (prompt == null)
            return;

        if (!isFocused)
        {
            prompt.Show(false);
            prompt.SetPulsing(false);
            return;
        }

        string text = state switch
        {
            PlotState.Empty => GetEmptyPromptText(),
            PlotState.Growing => "Growing...",
            PlotState.Ready => "Harvest",
            _ => ""
        };

        prompt.SetText(text);
        prompt.Show(true);
        prompt.SetPulsing(state == PlotState.Ready);
    }

    public bool MatchesCollider(Collider c)
    {
        if (c == null)
            return false;

        if (interactCollider != null)
            return c == interactCollider;

        return c.GetComponentInParent<CropPlot>() == this;
    }

    private void SetState(PlotState newState, bool resetTimer)
    {
        state = newState;

        if (resetTimer)
            growTimer = 0f;

        ApplyStateVisuals();
        ApplyCropGrowthVisual();
        RefreshPrompt();
    }

    private void ApplyStateVisuals()
    {
        if (plotRenderer == null)
            return;

        Color color = state switch
        {
            PlotState.Empty => new Color(0.25f, 0.15f, 0.08f),
            PlotState.Growing => new Color(0.35f, 0.22f, 0.12f),
            PlotState.Ready => new Color(0.25f, 0.35f, 0.12f),
            _ => Color.white
        };

        plotRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(ColorId, color);
        plotRenderer.SetPropertyBlock(mpb);
    }

    private void ApplyCropGrowthVisual()
    {
        if (cropVisual == null)
            return;

        bool shouldShow = state != PlotState.Empty;

        if (cropVisual.gameObject.activeSelf != shouldShow)
            cropVisual.gameObject.SetActive(shouldShow);

        if (!shouldShow)
            return;

        float t = state == PlotState.Ready
            ? 1f
            : Mathf.Clamp01(growTimer / Mathf.Max(0.0001f, currentGrowTime));

        cropVisual.localScale = Vector3.Lerp(cropScaleMin, cropScaleMax, t);
    }
}