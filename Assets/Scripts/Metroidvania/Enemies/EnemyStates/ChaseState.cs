using UnityEngine;

public class ChaseState : State
{
    private Transform target;
    protected override string AnimBoolName => "isRunning";
    public ChaseState(Enemy enemy) : base(enemy) {}

    public override void FixedUpdate()
    {
        //Check for target
        target = senses.GetChaseTarget();
        if(!target)
        {
            stateMachine.ChangeState(new PatrolState(enemy));
            return;
        }

        enemy.FaceTarget(target);

        //Check if target is in melee range
        if(senses.IsInMeleeRange(target) && combat.CanMeleeAttack())
        {
            stateMachine.ChangeState(new MeleeAttackState(enemy));
            return;
        }

        //Check if target is within turn threshold
        float distance = Mathf.Abs(target.position.x - enemy.transform.position.x);
        if(distance <= config.turnThreshold)
        {
            stateMachine.ChangeState(new IdleState(enemy));
            return;
        }

        //Checking for wall or cliff
        if(senses.IsHittingWall() || senses.IsAtCliff())
        {
            stateMachine.ChangeState(new IdleState(enemy));
            return;
        }

        //Move towards the target
        rb.linearVelocity = new Vector2(config.chaseSpeed * enemy.FacingDirection, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        rb.linearVelocity = Vector2.zero;
    }

}
