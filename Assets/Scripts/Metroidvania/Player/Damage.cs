using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private Player player;
    public Health health;

    [Header("Knockback Settings")]
    public float knockbackForce = 20;
    public float knockbackDuration = 0.2f;


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

        player.damagedState.SetParameters(knockBackDir);
        player.ChangeState(player.damagedState);
    }

    void HandleDeath(Vector2 sourcePosition)
    {
        int knockBackDir = 0;
        knockBackDir = transform.position.x > sourcePosition.x ? 1 : -1;

        player.deathState.SetParameters(knockBackDir);
        player.ChangeState(player.deathState);
    }
}
