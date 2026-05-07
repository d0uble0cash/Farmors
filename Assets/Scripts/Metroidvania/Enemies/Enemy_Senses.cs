using UnityEngine;

public class Enemy_Senses : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private EnemyConfig config;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;

    public bool IsAtCliff() => !Physics2D.Raycast(groundCheck.position, Vector2.down, config.groundCheckDistance, config.groundLayer);
    public bool IsHittingWall() => !Physics2D.Raycast(wallCheck.position, Vector2.down, config.wallCheckDistance, config.wallLayer);

    private void ODrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * config.groundCheckDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * enemy.FacingDirection * config.wallCheckDistance);
    }
}
