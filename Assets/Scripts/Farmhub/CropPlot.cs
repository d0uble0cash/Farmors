using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CropPlot : MonoBehaviour
{
    private enum PlotState { Empty, Growing, Ready }

    [Header("Settings")]
    [SerializeField] private float growTimeSeconds = 5f;
    [SerializeField] private int harvestMoney = 10;
    [SerializeField] private float interactDistance = 5f;

    [Header("Visuals")]
    [SerializeField] private Renderer plotRenderer;
    [SerializeField] private Transform cropVisual;
    [SerializeField] private Vector3 cropScaleMin = new(0.2f, 0.2f, 0.2f);
    [SerializeField] private Vector3 cropScaleMax = new(0.8f, 1.0f, 0.8f);

    [Header("UI Prompt")]
    [SerializeField] private TextMeshProUGUI promptText;

    private PlotState state = PlotState.Empty;
    private float growTimer;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;

        if (plotRenderer == null)
            plotRenderer = GetComponentInChildren<Renderer>();

        ApplyStateVisuals();
        ApplyCropGrowthVisual();
        SetPromptVisible(false);
    }

    private void Update()
    {
        TickGrowth();

        bool looking = IsLookingAtThisPlot();
        UpdatePrompt(looking);

        if (looking && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            Interact();
    }

    private void TickGrowth()
    {
        if (state != PlotState.Growing)
            return;

        growTimer += Time.deltaTime;

        if (growTimer >= growTimeSeconds)
        {
            growTimer = growTimeSeconds;
            state = PlotState.Ready;
            ApplyStateVisuals();
        }

        ApplyCropGrowthVisual();
    }

    private void Interact()
    {
        switch (state)
        {
            case PlotState.Empty:
                state = PlotState.Growing;
                growTimer = 0f;
                ApplyStateVisuals();
                ApplyCropGrowthVisual();
                break;

            case PlotState.Ready:
                GameState.I.AddMoney(harvestMoney);
                state = PlotState.Empty;
                growTimer = 0f;
                ApplyStateVisuals();
                ApplyCropGrowthVisual();
                break;
        }
    }

    private void UpdatePrompt(bool looking)
    {
        if (promptText == null)
            return;

        if (!looking)
        {
            SetPromptVisible(false);
            return;
        }

        SetPromptVisible(true);

        promptText.text = state switch
        {
            PlotState.Empty => "Press E to Plant",
            PlotState.Growing => "Growing...",
            PlotState.Ready => "Press E to Harvest",
            _ => ""
        };
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptText.gameObject.activeSelf != visible)
            promptText.gameObject.SetActive(visible);
    }

    private void ApplyStateVisuals()
    {
        if (plotRenderer == null)
            return;

        plotRenderer.material.color = state switch
        {
            PlotState.Empty => new Color(0.25f, 0.15f, 0.08f),
            PlotState.Growing => new Color(0.35f, 0.22f, 0.12f),
            PlotState.Ready => new Color(0.25f, 0.35f, 0.12f),
            _ => plotRenderer.material.color
        };
    }

    private void ApplyCropGrowthVisual()
    {
        if (cropVisual == null)
            return;

        bool shouldShow = state != PlotState.Empty;
        cropVisual.gameObject.SetActive(shouldShow);

        if (!shouldShow)
            return;

        float t = (state == PlotState.Ready)
            ? 1f
            : Mathf.Clamp01(growTimer / growTimeSeconds);

        cropVisual.localScale = Vector3.Lerp(cropScaleMin, cropScaleMax, t);
    }

    private bool IsLookingAtThisPlot()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return false;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            return hit.collider.GetComponentInParent<CropPlot>() == this;
        }

        return false;
    }
}