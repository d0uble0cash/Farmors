using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class FarmPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private NavMeshAgent agent;

    private Vector2 moveInput;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (cam == null)
            cam = Camera.main;
    }

    private void Update()
    {
        if (cam == null)
            return;

        bool isManualMoving = moveInput.sqrMagnitude > 0.01f;
        if (!isManualMoving)
            return;

        agent.ResetPath();

        Vector3 forward = cam.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cam.transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        if (moveDirection.sqrMagnitude > 0.01f)
            moveDirection.Normalize();

        agent.Move(moveDirection * agent.speed * Time.deltaTime);
    }
}