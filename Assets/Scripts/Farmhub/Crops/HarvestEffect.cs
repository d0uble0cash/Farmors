using UnityEngine;

public class HarvestEffect : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float popHeight = 0.6f;
    [SerializeField] private float spinSpeed = 180f;
    [SerializeField] private AnimationCurve scaleCurve;

    private Vector3 startPosition;
    private Vector3 startScale;
    private float timer;

    private void Awake()
    {
        startPosition = transform.position;
        startScale = transform.localScale;

        if (scaleCurve == null || scaleCurve.length == 0)
        {
            scaleCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.2f, 1.25f),
                new Keyframe(0.6f, 1f),
                new Keyframe(1f, 0f)
            );
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / lifetime);

        float height = Mathf.Sin(t * Mathf.PI) * popHeight;
        transform.position = startPosition + Vector3.up * height;

        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);

        float scale = scaleCurve.Evaluate(t);
        transform.localScale = startScale * scale;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}