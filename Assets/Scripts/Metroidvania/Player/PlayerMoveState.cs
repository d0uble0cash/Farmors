using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player) {}

    public override void Enter()
    {
        base.Enter();


    }


    public override void Update()
    {
        base.Update();

        if(AttackPressed && combat.CanAttack)
            player.ChangeState(player.attackState);

        else if (JumpPressed)
        {
            player.ChangeState(player.jumpState);
        }
    
        if(JumpPressed)
        {
            player.ChangeState(player.jumpState);
        }
        else if(Mathf.Abs(MoveInput.x) < 0.1f)
        {
            player.ChangeState(player.idleState);
        }
        else if(player.isGrounded && RunPressed && MoveInput.y < -.1f)
        {
            player.ChangeState(player.slideState);
        }
        else
        {
            animator.SetBool("isWalking", !RunPressed);
            animator.SetBool("isRunning", RunPressed);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float speed = RunPressed ? player.runSpeed : player.walkSpeed;
        rb.linearVelocity = new Vector2(speed * player.facingDirection, rb.linearVelocity.y);
    }
    public override void Exit()
    {
        base.Exit();

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }
}
