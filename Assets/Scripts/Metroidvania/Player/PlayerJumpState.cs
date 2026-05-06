using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player) : base(player) {}

    public override void Enter()
    {
        base.Enter();
        animator.SetBool("isJumping", true);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.jumpForce);
    
        JumpPressed = false;
        JumpReleased = false;
    }

    public override void Update()
    {
        base.Update();

        if(player.isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            if(Mathf.Abs(MoveInput.x) > 0.1f)
                player.ChangeState(player.moveState);
            else
                player.ChangeState(player.idleState);
        }
            
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.ApplyVariableGravity();

        if(JumpReleased && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            JumpReleased = false;
        }

        float speed = RunPressed ? player.runSpeed :player.walkSpeed;
        float targetSpeed = speed * MoveInput.x;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
        
    }

    public override void Exit()
    {
        base.Exit();
        animator.SetBool("isJumping", false);
    }
}

/*     private void HandleJump()
    {
        bool canJump = isGrounded || coyoteTimeCounter > 0f;
        bool jumpQueued = jumpBufferCounter > 0f;
        if(jumpQueued && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            jumpPressed = false;
            jumpReleased = false;
        } else if (jumpQueued && !canJump)
        {
            if(abilities != null &&  abilities.TryAbilityJump())
            {
                jumpBufferCounter = 0f;
            }
        }
        if(jumpReleased && rb.linearVelocity.y > 0f)
        {
            float cutSpeed = Mathf.Max(rb.linearVelocity.y * jumpCutMultiplier, jumpCutMinSpeed);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, cutSpeed);
            jumpReleased = false;
        }
    } */
