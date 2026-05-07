using UnityEngine;

public class PatrolState : State
{
    private float flipCooldown = .5f;
    private float lastFlipTime;
    public PatrolState(Enemy enemy) : base(enemy) {}
    protected override string AnimBoolName => "isWalking";

    public override void Enter()
    {
        base.Enter(); 
        lastFlipTime = Time.time;
    }


    public override void FixedUpdate()
    {
        if(senses.GetChaseTarget()) {
            stateMachine.ChangeState(new ChaseState(enemy));
            return;
        }

        rb.linearVelocity = new Vector2(config.patrolSpeed * enemy.FacingDirection, rb.linearVelocity.y);
        
        if(Time.time < lastFlipTime + flipCooldown) return;
        
        if(senses.IsHittingWall() || senses.IsAtCliff())
        {
            enemy.Flip();
            return;
        }
        
    }
}
