using UnityEngine;

public class BossDazedState : BossState
{
    protected override string AnimBoolName => "isDazed";

    private float dazeTimer;
    public BossDazedState(StagBoss boss) : base(boss){}

    public override void Enter()
    {
        base.Enter();
        rb.linearVelocity = Vector2.zero;
        dazeTimer = boss.dazeDuration;
        boss.SetGlow(boss.dazedColor);

        rb.AddForce(new Vector2(-boss.FacingDirection * 3f, 2f), ForceMode2D.Impulse);
    }

    public override void Update()
    {
        dazeTimer -= Time.deltaTime;

        float pulse = Mathf.PingPong(Time.time * 3f, 1f);
        boss.SetGlow(Color.Lerp(boss.dazedColor, Color.white, pulse));

        if(dazeTimer <= 0f)
        {
            stateMachine.ChangeState(boss.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        boss.SetGlow(boss.normalColor);
    }
}
