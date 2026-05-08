using UnityEngine;

public class Enemy_Senses : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private EnemyConfig config;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform attackPoint;

    public bool IsAtCliff() => !Physics2D.Raycast(groundCheck.position, Vector2.down, config.groundCheckDistance, config.groundLayer);
    public bool IsHittingWall() => Physics2D.Raycast(wallCheck.position, Vector2.right * enemy.FacingDirection, config.wallCheckDistance, config.wallLayer);

    public Transform GetChaseTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, config.chaseRange, config.targetLayer);
        if(!hit)
        {
            return null;
        }
        Player player = hit.GetComponent<Player>();
        if(player.currentState == player.deathState)
        {
            return null;
        }

        return hit.transform;
    }

    public bool IsInMeleeRange(Transform target)
    {
        if(!target)
        {
            return false;
        }
        float distance = Mathf.Abs(target.position.x - attackPoint.position.x);
        return distance <= config.meleeRange;
    }
    private void ODrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * config.groundCheckDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * enemy.FacingDirection * config.wallCheckDistance);
    
        //Chase Check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, config.chaseRange);
    
        //Melee Range Check
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, config.meleeRange);
    }
}
