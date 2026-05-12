using UnityEngine;

public class AnimalGrowth : MonoBehaviour
{
    [Header("Animal")]
    [SerializeField] private string speciesId = "sheep";
    [SerializeField] private bool startsAsBaby = false;

    [Header("Growth")]
    [SerializeField] private float growTimeSeconds = 60f;
    [SerializeField] private Vector3 babyScale = new Vector3(0.45f, 0.45f, 0.45f);
    [SerializeField] private Vector3 adultScale = Vector3.one;

    public string SpeciesId => speciesId;
    public bool IsAdult => !isBaby;

    private bool isBaby;
    private float growTimer;
    private bool wasInitializedAtRuntime;

    private void Start()
    {
        if (wasInitializedAtRuntime)
            return;

        if (startsAsBaby)
            InitializeAsBaby();
        else
            InitializeAsAdult();
    }

    private void Update()
    {
        if (!isBaby)
            return;

        growTimer += Time.deltaTime;

        float t = Mathf.Clamp01(growTimer / Mathf.Max(0.01f, growTimeSeconds));
        transform.localScale = Vector3.Lerp(babyScale, adultScale, t);

        if (growTimer >= growTimeSeconds)
        {
            InitializeAsAdult();
        }
    }

    public void InitializeAsBaby()
    {
        wasInitializedAtRuntime = true;
        isBaby = true;
        growTimer = 0f;
        transform.localScale = babyScale;
    }

    private void InitializeAsAdult()
    {
        wasInitializedAtRuntime = true;
        isBaby = false;
        growTimer = growTimeSeconds;
        transform.localScale = adultScale;
    }
}