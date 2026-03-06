using UnityEngine;
using TMPro;

public class WorldPrompt : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TextMeshPro text;

    [Header("Billboard")]
    [SerializeField] private bool faceCamera = true;

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 10f; // higher = snappier

    [Header("Bob")]
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private float bobFrequency = 2f;

    [Header("Pulse (when ready)")]
    [SerializeField] private float pulseAmount = 0.08f;
    [SerializeField] private float pulseFrequency = 3f;

    private Camera cam;

    private float targetAlpha = 0f;
    private float currentAlpha = 0f;

    private Vector3 baseLocalPos;
    private Vector3 baseLocalScale;

    private bool pulsing;

    private void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshPro>();

        cam = Camera.main;

        baseLocalPos = transform.localPosition;
        baseLocalScale = transform.localScale;

        // Start hidden without disabling the object
        SetAlphaImmediate(0f);
    }

    private void Update()
    {
        // Smooth fade (no coroutines needed)
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
        ApplyAlpha(currentAlpha);

        // Bob always (only noticeable when visible)
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.localPosition = baseLocalPos + new Vector3(0f, bob, 0f);

        // Pulse only when "Ready"
        if (pulsing)
        {
            float p = Mathf.Sin(Time.time * pulseFrequency) * 0.5f + 0.5f; // 0..1
            float s = 1f + (p * pulseAmount);
            transform.localScale = baseLocalScale * s;
        }
        else
        {
            transform.localScale = baseLocalScale;
        }

        // Billboard
        if (faceCamera)
        {
            if (cam == null) cam = Camera.main;
            if (cam != null)
            {
                // Face camera, but keep upright (optional: remove flattening if you want full look-at)
                Vector3 forward = cam.transform.forward;
                forward.y = 0f;
                if (forward.sqrMagnitude > 0.0001f)
                    transform.forward = forward.normalized;
            }
        }
    }

    public void SetText(string value)
    {
        if (text == null) return;
        if (text.text != value) text.text = value;
    }

    public void Show(bool show)
    {
        targetAlpha = show ? 1f : 0f;
    }

    public void SetPulsing(bool enabled)
    {
        pulsing = enabled;
    }

    private void ApplyAlpha(float a)
    {
        if (text == null) return;
        Color c = text.color;
        c.a = a;
        text.color = c;
    }

    private void SetAlphaImmediate(float a)
    {
        currentAlpha = a;
        targetAlpha = a;
        ApplyAlpha(a);
    }
}