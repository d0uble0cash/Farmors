using UnityEngine;

public class CropPlot : MonoBehaviour, IInteractable
{
    public enum PlotState
    {
        Empty,
        Growing,
        Ready
    }

    [Header("Settings")]
    [SerializeField] private float growTimeSeconds = 5f;

    [Header("Visuals")]
    [SerializeField] private Renderer plotRenderer;
    [SerializeField] private Transform cropVisual;
    [SerializeField] private Vector3 cropScaleMin = new(0.2f, 0.2f, 0.2f);
    [SerializeField] private Vector3 cropScaleMax = new(0.8f, 1.0f, 0.8f);

    [Header("World Prompt")]
    [SerializeField] private WorldPrompt prompt;

    [Header("Interaction")]
    [SerializeField] private Collider interactCollider;

    [Header("Crop Data")]
    [SerializeField] private ItemDefinition harvestItem;
    [SerializeField] private int harvestAmount = 1;
    [SerializeField] private ItemDefinition requiredSeed;

    public PlotState State => state;
    public bool CanInteract => state == PlotState.Empty || state == PlotState.Ready;

    private PlotState state = PlotState.Empty;
    private float growTimer = 0f;
    private bool isFocused = false;

    private MaterialPropertyBlock mpb;
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (plotRenderer == null)
        {
            plotRenderer = GetComponentInChildren<Renderer>();
        }

        if (interactCollider == null)
        {
            interactCollider = GetComponentInChildren<Collider>();
        }

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
        {
            return;
        }

        growTimer += Time.deltaTime;

        if (growTimer >= growTimeSeconds)
        {
            growTimer = growTimeSeconds;
            SetState(PlotState.Ready, false);
            return;
        }

        ApplyCropGrowthVisual();
    }

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

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
                PlantSeed();
                break;

            case PlotState.Ready:
                HarvestCrop();
                break;
        }
    }

    private void PlantSeed()
    {
        if (requiredSeed == null)
        {
            Debug.LogError("CropPlot requiredSeed is not set.", this);
            return;
        }

        if (!GameState.I.PlayerInventory.TryRemove(requiredSeed.Id, 1))
        {
            Debug.Log($"Not enough seeds: {requiredSeed.DisplayName}", this);
            RefreshPrompt();
            return;
        }

        SetState(PlotState.Growing, true);
    }

    private void HarvestCrop()
    {
        if (harvestItem == null)
        {
            Debug.LogError("CropPlot harvestItem is not set.", this);
            return;
        }

        GameState.I.PlayerInventory.Add(harvestItem.Id, harvestAmount);
        Debug.Log($"Harvested {harvestAmount}x {harvestItem.DisplayName}!", this);

        SetState(PlotState.Empty, true);
    }

    public void SetFocused(bool focused)
    {
        isFocused = focused;
        RefreshPrompt();
    }

    private void RefreshPrompt()
    {
        if (prompt == null)
        {
            return;
        }

        if (!isFocused)
        {
            prompt.Show(false);
            prompt.SetPulsing(false);
            return;
        }

        string text = state switch
        {
            PlotState.Empty => (GameState.I != null &&
                                GameState.I.PlayerInventory != null &&
                                requiredSeed != null &&
                                GameState.I.PlayerInventory.Has(requiredSeed.Id))
                ? "Plant"
                : "Need Seeds",

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
        {
            return false;
        }

        if (interactCollider != null)
        {
            return c == interactCollider;
        }

        return c.GetComponentInParent<CropPlot>() == this;
    }

    private void SetState(PlotState newState, bool resetTimer)
    {
        state = newState;

        if (resetTimer)
        {
            growTimer = 0f;
        }

        ApplyStateVisuals();
        ApplyCropGrowthVisual();
        RefreshPrompt();
    }

    private void ApplyStateVisuals()
    {
        if (plotRenderer == null)
        {
            return;
        }

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
        {
            return;
        }

        bool shouldShow = state != PlotState.Empty;

        if (cropVisual.gameObject.activeSelf != shouldShow)
        {
            cropVisual.gameObject.SetActive(shouldShow);
        }

        if (!shouldShow)
        {
            return;
        }

        float t = state == PlotState.Ready
            ? 1f
            : Mathf.Clamp01(growTimer / Mathf.Max(0.0001f, growTimeSeconds));

        cropVisual.localScale = Vector3.Lerp(cropScaleMin, cropScaleMax, t);
    }
}