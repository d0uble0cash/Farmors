using UnityEngine;

public class BossChargeState : BossState
{
    protected override string AnimBoolName => "isCharging";

    private float maxChargeDurection = 3f;
    private float chargeTimer;

    public BossChargeState(StagBoss boss) : base(boss){}

    public override void Enter()
    {
        base.Enter();
        chargeTimer = maxChargeDurection;
        boss.SetGlow(boss.chargeGlowColor);
    }

    public override void FixedUpdate()
    {
        chargeTimer -= Time.fixedDeltaTime;

        if(boss.IsHittingWall())
        {
            stateMachine.ChangeState(boss.dazedState);
            return;
        }

        if(chargeTimer <= 0f)
        {
            stateMachine.ChangeState(boss.idleState);
            return;
        }

        rb.linearVelocity = new Vector2(boss.chargeSpeed * boss.FacingDirection, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        rb.linearVelocity = Vector2.zero;
        boss.SetGlow(boss.normalColor);
    }
}
