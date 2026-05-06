using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage;
    public float attackRadius = .5f;
    public float attackCooldown = .5f;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public Player player;
    public bool CanAttack => Time.time >= nextAttackTime;
    private float nextAttackTime;

    public void AttackAnimationFinished()
    {
        player.AttackAnimationFinished();
    }

    public void Attack()
    {
        if(!CanAttack)
            return;

        nextAttackTime = Time.time + attackCooldown;

        Collider2D enemy = Physics2D.OverlapCircle(attackPoint.position, attackRadius, enemyLayer);
        if(enemy != null)
            enemy.gameObject.GetComponent<Health>().ChangeHealth(-damage);
    }
}
