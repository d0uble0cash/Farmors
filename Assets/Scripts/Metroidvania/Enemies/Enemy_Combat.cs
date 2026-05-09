using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    private EnemyConfig config;
    private Enemy enemy;
    private float lastAttackTime = -10f;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        config = enemy.Config;
    }
    
    public bool CanMeleeAttack() => Time.time >= lastAttackTime + config.meleeCooldown;
    public void PerformMeleeAttack()
    {
        lastAttackTime = Time.time;
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, config.meleeRange, config.targetLayer);
        if(!hit)
        {
            return;
        }
        Health health = hit.GetComponentInChildren<Health>();
        if(health != null)
        {
            health.ChangeHealth(-config.meleeDamage, transform.position);
        }
    }
}
