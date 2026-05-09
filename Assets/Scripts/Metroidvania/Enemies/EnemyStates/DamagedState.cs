using UnityEngine;

public class DamagedState : State
{
    protected override string AnimBoolName => "isHurt";
    private float knockbackVelocity;
    private float knockbackDuration;
    private float knockbackTimer;
    private float animationTimer;
    private bool knockbackApplied = false;
    public DamagedState(Enemy enemy, int knockbackDir) : base(enemy)
    {
        knockbackVelocity = knockbackDir * config.knockbackForce;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("DamagedState entered, isHurt: " + anim.GetBool("isHurt"));
        knockbackTimer = .55f;
        animationTimer = .55f;
        knockbackApplied = false;
        if(!senses.IsAtCliff())
            rb.linearVelocity = new Vector2(knockbackVelocity, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

    }
    

    public override void FixedUpdate()
    {
        if(!knockbackApplied)
        {
            rb.linearVelocity = new UnityEngine.Vector2(knockbackVelocity, rb.linearVelocity.y);
            knockbackApplied = true;
        }
        knockbackTimer -= Time.fixedDeltaTime;
        animationTimer -= Time.fixedDeltaTime;

        if(senses.IsAtCliff())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            knockbackTimer = 0f;
        }
        if(knockbackTimer <= 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            stateMachine.ChangeState(new IdleState(enemy));
        }
        if(animationTimer <= 0)
        {
            if(!senses.IsAtCliff())
                stateMachine.ChangeState(new IdleState(enemy));
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("DamagedState exited after: " + (config.knockbackDuration - knockbackTimer) + "s");
    }


}
