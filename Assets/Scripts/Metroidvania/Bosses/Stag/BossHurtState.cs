using UnityEngine;

public class BossHurtState : BossState
{

    protected override string AnimBoolName => "isHurt";
 
    private float hurtDuration = 0.3f;
    private float hurtTimer;
 
    public BossHurtState(StagBoss boss) : base(boss) {}
 
    public override void Enter()
    {
        base.Enter();
        hurtTimer = hurtDuration;
        rb.linearVelocity = Vector2.zero;
    }
 
    public override void Update()
    {
        hurtTimer -= Time.deltaTime;
        if (hurtTimer <= 0f)
            stateMachine.ChangeState(boss.dazedState);
    }
}
