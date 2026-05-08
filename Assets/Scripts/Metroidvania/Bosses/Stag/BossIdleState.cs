using NUnit.Framework;
using UnityEngine;

public class BossIdleState : BossState
{
    protected override string AnimBoolName => "isIdle";
    private float idleTimer;
    private float telegraphTimer;
    private bool isTelegraphing;
    public BossIdleState(StagBoss boss) : base(boss){}

    public override void Enter()
    {
        base.Enter();
        rb.linearVelocity = Vector2.zero;
        idleTimer = boss.idleDuration;
        telegraphTimer = boss.telegraphDuration;
        isTelegraphing = false;
        boss.SetGlow(boss.normalColor);
    }

    public override void Update()
    {
        boss.FacePlayer();

        if(!isTelegraphing)
        {
            idleTimer -= Time.deltaTime;
            anim?.SetBool("isTelegraphing", true);
            boss.SetGlow(boss.chargeGlowColor);
        }
        else
        {
            telegraphTimer -= Time.deltaTime;
            float pulse = Mathf.PingPong(Time.time *4f, 1f);
        
            boss.SetGlow(Color.Lerp(boss.normalColor, boss.chargeGlowColor, pulse));
        
            if(telegraphTimer <= 0f)
            {
                anim?.SetBool("isTelegraphing", false);
                stateMachine.ChangeState(boss.chargeState);
            }
        }
        
    }

}

