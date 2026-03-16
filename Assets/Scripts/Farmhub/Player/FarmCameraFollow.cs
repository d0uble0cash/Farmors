using UnityEngine;

public class FarmCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 5f;

    private void Awake()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned on FarmCameraFollow.");
        }
    }

    private void Start()
    {
        if (playerTransform != null)
        {
            offset = transform.position - playerTransform.position;
        }
    }
    private void LateUpdate()
    {
        if (playerTransform == null)
            return;

        Vector3 targetPosition = playerTransform.position + offset;
        targetPosition.y = transform.position.y;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}