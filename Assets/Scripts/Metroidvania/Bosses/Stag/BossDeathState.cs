using UnityEngine;

public class BossDeathState : BossState
{
    protected override string AnimBoolName => "isDead";
    
    private float deathDuration = 2f;
    private float deathTimer;
    private bool abilitySpawned = false;
    
    public BossDeathState(StagBoss boss) : base(boss) {}
    
    public override void Enter()
        {
            base.Enter();
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale   = 0f;
            deathTimer        = deathDuration;
            boss.SetGlow(Color.white);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            {
                PlayerAbilities abilities = player.GetComponent<PlayerAbilities>();
                if(abilities != null)
                {
                    abilities.UnlockAbility("walldash");
                }
            }
        }
    
    public override void Update()
    {
        deathTimer -= Time.deltaTime;

        if(boss.spriteRenderer != null)
        {
            Color c = boss.spriteRenderer.color;
            c.a = Mathf.Clamp01(deathTimer / deathDuration);
            boss.spriteRenderer.color = c;
        } 

        if(!abilitySpawned && deathTimer <= deathDuration)
        {
            abilitySpawned = true;
            boss.SpawnAbilityDrop();
        }

        if(deathTimer <= 0f)
        {
            GameObject.Destroy(boss.gameObject);    
        }
    }
}
