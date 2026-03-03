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

    [Header("Optional visuals")]
    [SerializeField] private Renderer plotRenderer;     // drag the cube's Renderer here
    [SerializeField] private Renderer cropRenderer;     // optional: a small cube/sprite that appears when planted

    [SerializeField] private TextMeshProUGUI promptText;

    private PlotState state = PlotState.Empty;
    private float growTimer = 0f;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        if (plotRenderer == null) plotRenderer = GetComponentInChildren<Renderer>();
        SetVisuals();
    }

    private void Update()
{
    // 1) Grow timer (this must always run while Growing)
    if (state == PlotState.Growing)
    {
        growTimer += Time.deltaTime;

        if (growTimer >= growTimeSeconds)
        {
            state = PlotState.Ready;
            SetVisuals();
            Debug.Log("Crop is ready!");
        }
    }

    // 2) Are we looking at this plot?
    bool looking = IsPlayerLookingAtThisPlot();

    // 3) Prompt UI
    if (promptText != null)
    {
        if (looking)
        {
            promptText.gameObject.SetActive(true);

            if (state == PlotState.Empty)
                promptText.text = "Press E to Plant";
            else if (state == PlotState.Ready)
                promptText.text = "Press E to Harvest";
            else
                promptText.text = "Growing...";
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    // 4) Interact
    if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && looking)
    {
        Interact();
    }
}

    private bool IsPlayerLookingAtThisPlot()
{
    if (cam == null) cam = Camera.main;
    if (cam == null) return false;

    Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.yellow);

    if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
    {
        // This makes it work even if you hit a child (like CropVisual)
        Debug.Log("Ray hit: " + hit.collider.name);
        return hit.collider.GetComponentInParent<CropPlot>() == this;
        
    }

    return false;
}

    private void Interact()
    {
        switch (state)
        {
            case PlotState.Empty:
                state = PlotState.Growing;
                growTimer = 0f;
                SetVisuals();
                Debug.Log("Planted!");
                break;

            case PlotState.Ready:
                // Harvest
                GameState.I.AddMoney(harvestMoney);
                Debug.Log($"Harvested! Money: {GameState.I.money}");

                state = PlotState.Empty;
                growTimer = 0f;
                SetVisuals();
                break;
        }
    }

    private void SetVisuals()
    {
        if (plotRenderer != null)
        {
            // Optional simple color feedback
            // Empty = dark soil, Growing = slightly lighter, Ready = greener/bright
            if (state == PlotState.Empty) plotRenderer.material.color = new Color(0.25f, 0.15f, 0.08f);
            if (state == PlotState.Growing) plotRenderer.material.color = new Color(0.35f, 0.22f, 0.12f);
            if (state == PlotState.Ready) plotRenderer.material.color = new Color(0.25f, 0.35f, 0.12f);
        }

        if (cropRenderer != null)
            cropRenderer.enabled = state != PlotState.Empty;
    }
}