using UnityEditor.Tilemaps;
using UnityEngine;

public class StagBoss : MonoBehaviour
{
    [Header("States")]
    public BossStateMachine StateMachine { get; private set; }
    public BossIdleState idleState;
    public BossChargeState chargeState;
    public BossDazedState dazedState;
    public BossHurtState hurtState; 
    public BossDeathState deathState;

    [Header("Components")]
    public Rigidbody2D RB { get; private set; }
    public Animator anim { get; private set; }
    public Health health;

    [Header("Movement")]
    public float chargeSpeed = 18f;
    public float idleDuration = 2f;
    public float telegraphDuration = 1.2f;

    [Header("Daze")]
    public float dazeDuration = 2.5f;

    [Header("Wall Detection")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.3f;
    public LayerMask wallLayer;

    [Header("Player Detection")]
    public Transform player;
    public float detectionRange = 15f;

    [Header("Ability Drop")]
    public GameObject dashPickup;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color chargeGlowColor = new Color(1, 0.3f, 0f, 1f);

    public Color normalColor = Color.white;
    public Color dazedColor = new Color(0.5f, 0.5f, 1f, 1f);

    //Runtime
    public int FacingDirection { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        StateMachine = new BossStateMachine();
        idleState = new BossIdleState(this);
        chargeState = new BossChargeState(this);
        dazedState = new BossDazedState(this);
        hurtState = new BossHurtState(this);
        deathState = new BossDeathState(this);
    }
    private void Start()
    {
        health.OnDamaged += OnDamaged;
        health.OnDeath += OnDeath;
        StateMachine.Initialize(idleState);

        if(player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if(p != null)
            {
                player = p.transform;
            }
        }
    }

    private void Update() => StateMachine.CurrentState?.Update();

    private void FixedUpdate() => StateMachine.CurrentState?.FixedUpdate();

    public bool IsHittingWall()
    {
        bool left = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer);
        bool right = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);
        return left || right;
    }

    public void FacePlayer()
    {
        if(player == null) return;
        int dir = player.position.x > transform.position.x ? 1: -1;
        if(dir != FacingDirection) Flip();
    }

    public void Flip()
    {
        FacingDirection *= -1;
        Vector3 scale = transform.localScale;
        scale.x = FacingDirection;
        transform.localScale = scale;
    }

    public void SetGlow(Color color)
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
    
    public void SpawnAbilityDrop()
    {
        if(dashPickup != null)
        {
            Instantiate(dashPickup, transform.position, Quaternion.identity);
        }
    }

    private void OnDamaged(Vector2 hitDirection)
    {
        if(IsDead) return;
        if(StateMachine.CurrentState is BossDazedState)
        {
            StateMachine.ChangeState(hurtState);
        }
    }

    private void OnDeath()
    {
        IsDead = true;
        StateMachine.ChangeState(deathState);
    }

    private void OnDestroy()
    {
        if(health != null)
        {
            health.OnDamaged -= OnDamaged;
            health.OnDeath -= OnDeath;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (wallCheckLeft  != null) Gizmos.DrawWireSphere(wallCheckLeft.position,  wallCheckRadius);
        if (wallCheckRight != null) Gizmos.DrawWireSphere(wallCheckRight.position, wallCheckRadius);
 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }


}
