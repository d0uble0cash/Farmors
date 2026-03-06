using UnityEngine;

public class CropPlot : MonoBehaviour
{
    public enum PlotState { Empty, Growing, Ready }

    [Header("Settings")]
    [SerializeField] private float growTimeSeconds = 5f;
    [SerializeField] private int harvestMoney = 10;

    [Header("Visuals")]
    [SerializeField] private Renderer plotRenderer;
    [SerializeField] private Transform cropVisual;
    [SerializeField] private Vector3 cropScaleMin = new(0.2f, 0.2f, 0.2f);
    [SerializeField] private Vector3 cropScaleMax = new(0.8f, 1.0f, 0.8f);

    [Header("World Prompt")]
    [SerializeField] private WorldPrompt prompt;

    [Header("Interaction")]
    [Tooltip("Optional: set a specific collider used for raycast hit testing. If null, any collider on this object/children works.")]
    [SerializeField] private Collider interactCollider;

    public PlotState State => state;
    public bool CanInteract => state == PlotState.Empty || state == PlotState.Ready;

    private PlotState state = PlotState.Empty;
    private float growTimer;
    private bool isFocused;

    private MaterialPropertyBlock mpb;
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (plotRenderer == null)
            plotRenderer = GetComponentInChildren<Renderer>();

        if (interactCollider == null)
            interactCollider = GetComponentInChildren<Collider>();

        mpb = new MaterialPropertyBlock();

        SetState(PlotState.Empty, resetTimer: true);

        // Start hidden until focused
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

    private void TickGrowth()
    {
        if (state != PlotState.Growing)
            return;

        growTimer += Time.deltaTime;

        if (growTimer >= growTimeSeconds)
        {
            growTimer = growTimeSeconds;
            SetState(PlotState.Ready, resetTimer: false);
            return;
        }

        ApplyCropGrowthVisual();
    }

    public void Interact()
    {
        if (!CanInteract)
            return;

        if (state == PlotState.Empty)
        {
            SetState(PlotState.Growing, resetTimer: true);
        }
        else if (state == PlotState.Ready)
        {
            if (GameState.I == null)
            {
                Debug.LogError("GameState.I is null. Start from Boot scene so Core singletons exist.");
                return;
            }

            GameState.I.AddMoney(harvestMoney);
            Debug.Log("Money: " + GameState.I.money);

            SetState(PlotState.Empty, resetTimer: true);
        }
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
            PlotState.Empty => "Plant",
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
        if (c == null) return false;

        if (interactCollider != null)
            return c == interactCollider || c.transform.IsChildOf(interactCollider.transform);

        return c.GetComponentInParent<CropPlot>() == this;
    }

    private void SetState(PlotState newState, bool resetTimer)
    {
        state = newState;

        if (resetTimer)
            growTimer = 0f;

        ApplyStateVisuals();
        ApplyCropGrowthVisual();
        RefreshPrompt(); // <-- THIS is what makes the prompt switch immediately
    }

    private void ApplyStateVisuals()
    {
        if (plotRenderer == null)
            return;

        Color c = state switch
        {
            PlotState.Empty => new Color(0.25f, 0.15f, 0.08f),
            PlotState.Growing => new Color(0.35f, 0.22f, 0.12f),
            PlotState.Ready => new Color(0.25f, 0.35f, 0.12f),
            _ => Color.white
        };

        plotRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(ColorId, c);
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

        float t = (state == PlotState.Ready)
            ? 1f
            : Mathf.Clamp01(growTimer / Mathf.Max(0.0001f, growTimeSeconds));

        cropVisual.localScale = Vector3.Lerp(cropScaleMin, cropScaleMax, t);
    }
}