using UnityEngine;

public class IdleState : State
{
    private Transform target;
    protected override string AnimBoolName => "isIdling";
    public IdleState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        base.Enter();
        rb.linearVelocity = Vector2.zero;
    }

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

        if(target != null && senses.IsInMeleeRange(target) && combat.CanMeleeAttack())
        {
            stateMachine.ChangeState(new MeleeAttackState(enemy));
            return;
        }

        //Check if we reached the target
        if(senses.IsInMeleeRange(target))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        //Obstacle detection
        if(senses.IsHittingWall() || senses.IsAtCliff())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        //Switch to chase state
        stateMachine.ChangeState(new ChaseState(enemy));
    }
}
