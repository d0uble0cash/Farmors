using UnityEngine;

public class FarmAnimationDriver : MonoBehaviour
{
    [SerializeField] private Animator visualAnimator;

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (visualAnimator == null)
            return;

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        float speed = distanceMoved / Mathf.Max(Time.deltaTime, 0.0001f);

        visualAnimator.SetFloat("Speed", speed);

        lastPosition = transform.position;
    }
}