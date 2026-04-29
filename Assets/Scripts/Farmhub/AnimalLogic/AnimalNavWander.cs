using UnityEngine;
using UnityEngine.AI;

public class AnimalNavWander : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform visualRoot;

    [Header("Wander")]
    [SerializeField] private float wanderRadius = 4f;
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;

    [Header("Movement")]
    [SerializeField] private float minMoveDistance = 1f;

    [Header("Bob")]
    [SerializeField] private float bobSpeed = 8f;
    [SerializeField] private float bobAmount = 0.03f;

    private Vector3 homePosition;
    private float waitTimer;
    private bool isWaiting = true;

    private Vector3 visualStartLocalPos;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Start()
    {
        homePosition = transform.position;

        if (visualRoot != null)
        {
            visualStartLocalPos = visualRoot.localPosition;
        }

        BeginWaiting();
    }

    private void Update()
    {
        if (agent == null)
        {
            return;
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                TryPickNewDestination();
            }

            UpdateVisualMotion(false);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            BeginWaiting();
        }

        bool isMoving = agent.remainingDistance > agent.stoppingDistance + 0.05f;

        UpdateVisualMotion(isMoving);
    }

    private void TryPickNewDestination()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = new Vector3(
                homePosition.x + randomCircle.x,
                homePosition.y,
                homePosition.z + randomCircle.y
            );

            if (Vector3.Distance(transform.position, candidate) < minMoveDistance)
            {
                continue;
            }

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                isWaiting = false;
                return;
            }
        }

        BeginWaiting();
    }

    private void BeginWaiting()
    {
        isWaiting = true;
        waitTimer = Random.Range(minWaitTime, maxWaitTime);

        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }

    private void UpdateVisualMotion(bool isMoving)
    {
        if (visualRoot == null)
        {
            return;
        }

        if (!isMoving)
        {
            // smoothly return to original position
            visualRoot.localPosition = Vector3.Lerp(
                visualRoot.localPosition,
                visualStartLocalPos,
                Time.deltaTime * 6f
            );
            return;
        }

        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        visualRoot.localPosition = visualStartLocalPos + new Vector3(0f, bob, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            Application.isPlaying ? homePosition : transform.position,
            wanderRadius
        );
    }
}