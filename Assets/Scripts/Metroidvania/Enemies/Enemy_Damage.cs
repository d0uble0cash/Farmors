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
        int knockBackDir = 0;
        knockBackDir = transform.position.x > sourcePosition.x ? 1 : -1;

        enemy.StateMachine.ChangeState(new DamagedState(enemy, knockBackDir));
    }

    void HandleDeath(Vector2 sourcePosition)
    {
        if(useDeathAnimation) 
        {
            DeathWithAnimation();
        }
        else
        {
            DeathWithParts();
        }
    }

    public void DeathWithAnimation()
    {
        Collider2D col = GetComponent<Collider2D>();
        if(col != null) col.enabled =false;

        if(enemy.RB != null)
        {
            enemy.RB.linearVelocity = Vector2.zero;
            enemy.RB.gravityScale = 0f;
        }

        enemy.Anim.SetBool("IsDead", true);
        enemy.enabled = false;
        Destroy(enemy.gameObject, deathAnimationDuration);
    }

    public void DeathWithParts()
    {
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
        Destroy(gameObject);
    }
}
