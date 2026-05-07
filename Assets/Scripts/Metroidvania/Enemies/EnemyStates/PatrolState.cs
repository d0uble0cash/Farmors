using UnityEngine;

public class PatrolState : State
{
    public PatrolState(Enemy enemy) : base(enemy) {}
    protected override string AnimBoolName => "isWalking";

    public override void FixedUpdate()
    {
        if(senses.GetChaseTarget()) {
            stateMachine.ChangeState(new ChaseState(enemy));
            return;
        }
        if(senses.IsHittingWall() || senses.IsAtCliff())
        {
            enemy.Flip();
            return;
        }
        rb.linearVelocity = new Vector2(config.patrolSpeed * enemy.FacingDirection, rb.linearVelocity.y);
    }
}
