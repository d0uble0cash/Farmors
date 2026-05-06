using UnityEngine;
using UnityEngine.AI;

public class AnimalNavWander : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Transform player;

    [Header("Wander")]
    [SerializeField] private float wanderRadius = 4f;
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;

    [Header("Movement")]
    [SerializeField] private float minMoveDistance = 1f;
    [SerializeField] private float idleTurnSpeed = 2f;

    [Header("Player")]
    [SerializeField] private float playerStopDistance = 1.5f;

    [Header("Bob")]
    [SerializeField] private float bobSpeed = 8f;
    [SerializeField] private float bobAmount = 0.03f;

    private Vector3 homePosition;
    private float waitTimer;
    private bool isWaiting = true;

    private Vector3 visualStartLocalPos;
    private Quaternion idleTargetRotation;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    private void Start()
    {
        homePosition = transform.position;

        if (visualRoot != null)
            visualStartLocalPos = visualRoot.localPosition;

        PickIdleRotation();
        BeginWaiting();
    }

    private void Update()
    {
        if (agent == null)
            return;

        if (IsPlayerNearby())
        {
            agent.ResetPath();
            isWaiting = true;
            UpdateVisualMotion(false);
            TurnWhileIdle();
            return;
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            TurnWhileIdle();
            UpdateVisualMotion(false);

            if (waitTimer <= 0f)
            {
                TryPickNewDestination();
            }

            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            BeginWaiting();
            return;
        }

        bool isMoving = agent.remainingDistance > agent.stoppingDistance + 0.05f;
        UpdateVisualMotion(isMoving);
    }

    private bool IsPlayerNearby()
    {
        if (player == null)
            return false;

        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        return sqrDistance <= playerStopDistance * playerStopDistance;
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
                continue;

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
        PickIdleRotation();

        if (agent != null && agent.isOnNavMesh)
            agent.ResetPath();
    }

    private void PickIdleRotation()
    {
        float randomY = Random.Range(0f, 360f);
        idleTargetRotation = Quaternion.Euler(0f, randomY, 0f);
    }

    private void TurnWhileIdle()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            idleTargetRotation,
            idleTurnSpeed * Time.deltaTime
        );
    }

    private void UpdateVisualMotion(bool isMoving)
    {
        if (visualRoot == null)
            return;

        if (!isMoving)
        {
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

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerStopDistance);
    }
}