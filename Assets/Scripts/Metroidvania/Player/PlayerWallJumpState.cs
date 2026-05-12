using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    private float horizontalJumpPercent = .5f;
    public PlayerWallJumpState(Player player) : base(player) {}

    public override void Enter()
    {
        animator.SetBool("isWallJumping", true);

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(-player.facingDirection * horizontalJumpPercent, 1f) * player.jumpForce;
    
        JumpPressed = false;
        JumpReleased = false;
    }

    public override void Update()
    {
        if(!player.isGrounded && player.isTouchingWall && MoveInput.x == player.facingDirection && rb.linearVelocity.y < 0)
        {
            player.ChangeState(player.wallSlideState);
        }

        else if(JumpPressed && player.isTouchingWall)
        {
            player.ChangeState(player.jumpState);
        }
        else if(player.isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        player.ApplyVariableGravity();

        if(JumpReleased && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * player.jumpForce);
            JumpReleased = false;
        }
    }

    public override void Exit()
    {
        animator.SetBool("isWallJumping", false);
    }

}
