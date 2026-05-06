using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Player))]
public class PlayerAbilities : MonoBehaviour
{
    [Header("Unlocked Abilities (use UnlockAbility() in GameObject - Dont set manually in Product)")]
    public bool hasDash = false;
    public bool hasDoubleJump = false;
    public bool hasWallJump = false;
    public bool hasWallDash = false;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.6f;

    [Header("Wall Dash")]
    public float wallDashForce = 25f;
    public float wallDashDuration = 0.2f;
    public float wallDashCooldown = 0.8f;
    public float wallDashVerticalBoost = 5f;

    [Header("Double Jump")]
    public float doubleJumpForce = 14f;

    [Header("Wall Jump")]
    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 14f;
    public float wallCheckDistance = 0.3f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;

    private Player player;
    private Rigidbody2D rb;
    private Animator animator;

    public bool isDashing
    {
        get;
        private set;
    }
    = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    public bool isWallDashing { get; private set; } = false;
    private float wallDashTimer = 0f;
    private float wallDashCooldownTimer = 0f;
    private bool usedDoubleJump = false;
    public bool isTouchingWall
    {
        get;
        private set;
    }
    = false;
    public bool isWallSliding
    {
        get;
        private set;
    }
    = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update() => TickTimers();
    private void FixedUpdate()
    {
        CheckWall();
        HandleWallSlide();
        if(player.isGrounded)
        {
            usedDoubleJump = false;
        }
    }

    private void TickTimers()
    {
        if(dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        if(wallDashCooldownTimer > 0f)
        {
            wallDashCooldownTimer -= Time.deltaTime;
        }
        if(isDashing)
        {
            dashTimer -= Time.deltaTime;
            if(dashTimer <= 0f)
            {
                EndDash();
            }
        }
        if(isWallDashing)
        {
            wallDashTimer -= Time.deltaTime;
            if(wallDashTimer <= 0f)
            {
                EndWallDash();
            }
        }
    }
    //Normal Dash
    private void StartDash()
    {
        if(!hasDash || isDashing || dashCooldownTimer > 0f)
        {
            return;

        }
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        rb.gravityScale = 0f;
        animator?.SetTrigger("Dash");
    }
    private void EndDash() => isDashing = false;
    //Player dashes from the wall horizontally 
    private void StartWallDash()
    {
        if(!hasWallDash || isWallDashing || wallDashCooldownTimer > 0f || !isTouchingWall)
        {
            return;
        }
        isWallDashing = true;
        wallDashTimer = wallDashDuration;
        wallDashCooldownTimer = wallDashCooldown;
        float dashDirection = -Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(dashDirection * wallDashForce, wallDashVerticalBoost);
        rb.gravityScale = 0f;
        animator?.SetTrigger("wallDash");
    }

    private void EndWallDash() => isWallDashing = false;

    //DOUBLE JUMP
    private bool TryDoubleJump()
    {
        if(!hasDoubleJump || usedDoubleJump || player.isGrounded)
        {
            return false;
        }
        usedDoubleJump = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
        animator?.SetTrigger("doubleJump");
        return true;
    }
    //WALL JUMP + SLIDE
    private void CheckWall()
    {
        if(!hasWallJump && !hasWallDash)
        {
            return;
        }
        isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, wallCheckDistance, wallLayer);
    }

    private void HandleWallSlide()
    {
        isWallSliding = hasWallJump && isTouchingWall && !player.isGrounded && rb.linearVelocity.y < 0f;
        if(isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
    }

    private bool TryWallJump()
    {
        if(!hasWallJump || !isTouchingWall || player.isGrounded)
        {
            return false;
        }
        rb.linearVelocity = new Vector2(-transform.localScale.x * wallJumpForceX, wallJumpForceY);
        return true;
    }

    //Try to use an ability jump when normal + coyote jump both fail.
    //Wall Jump takes priority over double jump if both are available and conditions are met.    
    public bool TryAbilityJump()
    {
        if(TryWallJump())
        {
            return true;
        }
        if(TryDoubleJump())
        {
            return true;
        }
        return false;
    }

    //Unlocks an ability
    public void UnlockAbility(string abilityName)
    {
        switch(abilityName.ToLower())
        {
            case "dash":
                hasDash = true;
                break;
            case "doublejump":
                hasDoubleJump = true;
                break;
            case "walljump":
                hasWallJump = true;
                break;
            case "walldash":
                hasWallDash = true;
                break;
            default:
                Debug.LogWarning($"Unknown ability: {abilityName}");
                return;
        }
        Debug.Log($"Ability Unlocked: {abilityName}");
        //Triggers the unlock action and UI notification
    }
    //Input as a dash button depending on wall or ground state
    public void OnDash(InputValue value)
    {
        if(!value.isPressed)
        {
            return;
        }
        if(hasWallDash && isTouchingWall)
        {
            StartWallDash();
        }
        else
        {
            StartDash();
        }
    }
    //Gizmos
    public void OnDrawGizmosSelected()
    {
        float dir = Application.isPlaying ? transform.localScale.x : 1f;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * dir * wallCheckDistance);
    }

}


