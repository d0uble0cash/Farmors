using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FarmPlayerController : MonoBehaviour{
    [Header("Raycast")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private NavMeshAgent agent;

    [Header("Distance")]
    [SerializeField] private float interactDistance = 100f;
    [SerializeField] private float sampleRadius = 1f;
    [SerializeField] private bool useCameraFarClip = true;
    private Vector2 moveInput;

    public void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        
        if (cam == null) {
            cam = Camera.main;
        }

        if (useCameraFarClip && cam != null) {  
            interactDistance = cam.farClipPlane;
        }
    }   

    private void Update() {
        bool isManualMoving = moveInput.sqrMagnitude > 0.01f;

        if (cam == null) {
            return;
        }   
        if (isManualMoving) {

            agent.ResetPath();
            Vector3 forward = cam.transform.forward;
            forward.y = 0;
            Vector3 right = cam.transform.right;
            right.y = 0;
            Vector3 moveDirection = forward.normalized * moveInput.y + right.normalized * moveInput.x;

            if (moveDirection.sqrMagnitude > 0.01f) {
                moveDirection = moveDirection.normalized;
            }
            agent.Move(agent.speed * Time.deltaTime * moveDirection);
            return;

        }

        if (Mouse.current == null ||!Mouse.current.leftButton.wasPressedThisFrame) {
            return;
        }

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance, groundMask)) {
            return;
        }

        if(NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, sampleRadius, NavMesh.AllAreas)){
            agent.SetDestination(navHit.position);
        }
    }
}