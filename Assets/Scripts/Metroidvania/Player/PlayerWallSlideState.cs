using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    private float wallSlideSpeed = -2f;
    public PlayerWallSlideState(Player player) : base(player) {}

    public override void Enter()
    {
        animator.SetBool("isWallSliding", true);
    }

    public override void Update()
    {
        if(JumpPressed)
        {
            JumpPressed = false;
            player.ChangeState(player.wallJumpState);
        }

        else if(!player.isTouchingWall || Mathf.Abs(MoveInput.x) < .1f)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(0, wallSlideSpeed);
        player.ApplyVariableGravity();
    }

    public override void Exit()
    {
        animator.SetBool("isWallSliding", false);
    }
}
