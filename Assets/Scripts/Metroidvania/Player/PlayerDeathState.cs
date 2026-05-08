using UnityEngine;

public class PlayerDeathState : PlayerState
{
    private float knockbackVelocity;
    private float knockbackDuration;
    private bool isTimeSlow;
    public PlayerDeathState(Player player) : base(player){}

    public override void Enter()
    {
        base.Enter();
        Time.timeScale = .3f;
        isTimeSlow = true;
        animator.SetBool("IsDead", true);

        player.groundCheckRadius = .2f;
        knockbackDuration = damage.knockbackDuration;
        player.rb.linearVelocity = new Vector2(knockbackVelocity, player.rb.linearVelocity.y);
    }

    public void SetParameters(int knockbackDirection)
    {
        knockbackVelocity = knockbackDirection * damage.knockbackForce;
    }

    public override void FixedUpdate()
    {
        knockbackDuration -= Time.fixedDeltaTime;
        if (knockbackDuration <= 0)
        {
            if (isTimeSlow)
            {
                Time.timeScale = 1f;
                isTimeSlow = false;
            }
            if(player.isGrounded)
                player.rb.linearVelocity = Vector2.zero;
        }
    }

    public override void Exit()
    {
        base.Exit();
        animator.SetBool("IsDead", false);
        player.groundCheckRadius = .35f;
    }
}
