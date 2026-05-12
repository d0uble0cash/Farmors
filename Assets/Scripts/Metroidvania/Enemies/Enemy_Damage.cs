using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    public Health health;

    [Header("Death Style")]
    public bool useDeathAnimation = false;
    public float deathAnimationDuration = 2.0f;

    [Header("Death FX Pieces")]
    [SerializeField] private GameObject[] deathParts;
    [SerializeField] private float spawnForce = 5;
    [SerializeField] private float torque = 5;
    [SerializeField] private float lifetime = 2;

    [SerializeField] private MonsterSeedReward seedReward;

    private void OnEnable() 
    {
        health.OnDamaged += HandleDamage;
        health.OnDeath += HandleDeath;
    }

    private void OnDisable() 
    {
        health.OnDamaged -= HandleDamage;
        health.OnDeath -= HandleDeath;
    }

    void HandleDamage(Vector2 sourcePosition)
    {
        Debug.Log("Enemy took damage from: " + sourcePosition);
        
        if(enemy.StateMachine.CurrentState is DamagedState) return;
        int knockBackDir = 0;
        knockBackDir = transform.position.x > sourcePosition.x ? 1 : -1;

        enemy.StateMachine.ChangeState(new DamagedState(enemy, knockBackDir));
    }

    void HandleDeath(Vector2 sourcePosition)
    {
        GiveSeedReward();

        if(useDeathAnimation) 
        {
            DeathWithAnimation();
        }
        else
        {
            DeathWithParts();
        }
    }
    private void GiveSeedReward()
    {
        Debug.Log("GiveSeedReward was called on " + name);
        if (seedReward == null) {
            seedReward = GetComponent<MonsterSeedReward>();
            Debug.LogWarning("Enemy_Damage: No MonsterSeedReward found on " + name);
            return;
        }
        Debug.Log("Enemy_Damage: Calling GiveRewards on " + seedReward.name);

        seedReward.GiveRewards();
    }

    public void DeathWithAnimation()
    {
        Debug.Log("DeathWithAnimation called, IsDead param exists: " + HasParameter("IsDead"));
        Collider2D col = GetComponent<Collider2D>();
        if(col != null) col.enabled = false;

        if(enemy.RB != null)
        {
            enemy.RB.linearVelocity = Vector2.zero;
            enemy.RB.gravityScale = 0f;
        }

        enemy.StateMachine.CurrentState?.Exit();
        enemy.Anim.SetBool("IsDead", true);
        enemy.enabled = false;
        Debug.Log("IsDead set to: " + enemy.Anim.GetBool("IsDead"));
        Destroy(enemy.gameObject, deathAnimationDuration);
    }

    private bool HasParameter(string paramName)
    {
        foreach(var param in enemy.Anim.parameters)
            if(param.name == paramName) return true;
        return false;
    }

    public void DeathWithParts()
    {
        enemy.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        enemy.RB.linearVelocity = Vector2.zero;
        foreach(GameObject prefab in deathParts)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0.5f, 1)).normalized;
            GameObject part = Instantiate(prefab, transform.position, rotation);
        
            Rigidbody2D rb = part.GetComponent<Rigidbody2D>();

            if(rb!= null) 
            {
                Vector2 randomDirection = new Vector2(Random.Range(-1,1), Random.Range(.5f, 1)).normalized;
                rb.linearVelocity = randomDirection * spawnForce;
                rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);
            }
            Destroy(part, lifetime);
        }
        Destroy(enemy.gameObject, 0.1f);
    }
}
