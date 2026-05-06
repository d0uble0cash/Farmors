using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public PlayerState currentState;
    public PlayerIdleState idleState;
    public PlayerJumpState jumpState;
    public PlayerMoveState moveState;
    public PlayerCrouchState crouchState;
    public PlayerSlideState slideState;
    public enum GameMode
    {
        Platformer,
        TopDown
    }

    [Header("Mode")]
    public GameMode currentMode = GameMode.Platformer;
    [Header("Components")]
   public Rigidbody2D rb;
   public PlayerInput playerInput;
   public Animator animator;
   public CapsuleCollider2D playerCollider;

   [Header("Movement Variables")]
   public float walkSpeed;
   public float runSpeed;
   public float topDownSpeed = 5f;
   [Header("Jump")]
   public float jumpForce = 16f;
   public float jumpCutMinSpeed = 2f;
   public float jumpCutMultiplier = 0.4f;
   [Header("Gravity")]
   public float normalGravity = 3f;
   public float fallGravity = 6f;
   public float jumpGravity = 2.5f;
   public int facingDirection = 1;

   [Header("Crouch Check")]
   public Transform headCheck;
   public float headCheckRadius = .2f;

   [Header("Slide Settings")]
    public float slideDuration = .6f;
    public float slideSpeed = 12;
    public float slideStopDuration = .15f;

    public float slideHeight;
    public Vector2 slideOffset;
    public float normalHeight;
    public Vector2 normalOffset;

    private bool isSliding;

   [Header("Coyote Time")]
   public float coyoteTimeDuration = 0.12f;

   [Header("Jump Buffer")]
   public float jumpBufferDuration = 0.12f;
   
   //Inputs
   public Vector2 moveInput;
   public bool runPressed;
   public bool jumpPressed;
   public bool jumpReleased;

   [Header("Ground Check")]
   public Transform groundCheck;
   public float groundCheckRadius = 0.15f;
   public LayerMask groundLayer;

   [Header("Grounded")]
   public bool isGrounded;
   public bool wasGrounded;

   private float coyoteTimeCounter;

   private float jumpBufferCounter;

   private PlayerAbilities abilities;

   [Header("Farm Components")]
   public PlayerInteractor farmInteractor;

   [Header("Cameras")]
   public GameObject cameraFarm;
   public GameObject cameraMetroid;


    private void Awake()
    {
        abilities = GetComponent<PlayerAbilities>();
        idleState = new PlayerIdleState(this);
        jumpState = new PlayerJumpState(this);
        moveState = new PlayerMoveState(this);
        crouchState = new PlayerCrouchState(this);
        slideState = new PlayerSlideState(this);

    }


    private void Start()
   {
        if (rb != null) rb.gravityScale = normalGravity;
        ChangeState(idleState);
   }


   void Update()
    {
        currentState.Update();
        if (!isSliding)
        {
            Flip();
        }
        HandleAnimations();
        TickTimers();
    }

   void FixedUpdate()
    {
        CheckGrounded();
        currentState.FixedUpdate();
        if (currentMode == GameMode.Platformer)
        {   
        } else
        {
            rb.gravityScale = 0f;
            HandleTopDownMovement();
        }
    }

    public void ChangeState (PlayerState newState)
    {
        if(currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }
    //Method to switch between platformer and farming mode
    public void SwitchMode(GameMode newMode)
    {
        currentMode = newMode;
        if (newMode == GameMode.TopDown)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;

            //Camera Switch
            if (cameraFarm != null) cameraFarm.SetActive(true);
            if (cameraMetroid != null) cameraMetroid.SetActive(false);

            //Component State
            if (farmInteractor != null) farmInteractor.enabled = true;
            if (abilities != null) abilities.enabled = false;
        } else
        {
            rb.gravityScale = normalGravity;

            //Camera
            if (cameraFarm != null) cameraFarm.SetActive(false);
            if (cameraMetroid != null) cameraMetroid.SetActive(true);

            //Components
            if (farmInteractor != null) farmInteractor.enabled = false;
            if (abilities != null) abilities.enabled = true;
        }
        animator?.SetInteger("GameMode", (int)newMode);
    }

    private void TickTimers()
    {
        if (wasGrounded && !isGrounded)
        {
            coyoteTimeCounter = coyoteTimeDuration;
        }
        else  if (!isGrounded)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferDuration;
        } else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void HandlePlatformerMovement()
    {
        float targetSpeed = moveInput.x * walkSpeed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }



    public void SetColliderNormal()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, normalHeight);
        playerCollider.offset = normalOffset;
    }

    public void SetColliderSlide()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        playerCollider.offset = slideOffset;
    }



    public void ApplyVariableGravity()
    {
        if(abilities != null && (abilities.isDashing || abilities.isWallDashing))
        {
            return;
        }
        if(rb.linearVelocity.y < -0.1f) //falling
        {
            rb.gravityScale = fallGravity;
        }
        else if(rb.linearVelocity.y > 0.1f) //jumping
        {
            rb.gravityScale = jumpGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    private void HandleTopDownMovement()
    {
        if (rb == null) return; //null check
        rb.linearVelocity = moveInput.normalized * topDownSpeed;
    }

    void CheckGrounded()
    {
        //FIX : null check so missing GroundCheck doesn't crash the game
        if (groundCheck == null) return;
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool CheckForCeiling()
    {
        return Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
    }

    void HandleAnimations()
    {
        //FIX : null check so missing Animator doesn't crash the game
        if (animator == null) return;
        if(currentMode == GameMode.Platformer) {
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
            
        }
        else
        {
            animator.SetFloat("moveX", moveInput.x);
            animator.SetFloat("moveY", moveInput.y);
            animator.SetBool("isMoving", moveInput.sqrMagnitude > 0.1f);
        }
    }

    private void Flip()
    {
        if(currentMode == GameMode.TopDown) 
        {
            return;
        }
        if(moveInput.x > 0.1f)
        {
            facingDirection = 1;
        }
        else if(moveInput.x < -0.1f)
        {
            facingDirection = -1;
        }
        transform.localScale = new Vector3(facingDirection, 1, 1);
    }




    public void OnMove (InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnSprint (InputValue value)
    {
        runPressed = value.isPressed;
    }

    public void OnJump (InputValue value)
    {
        if(value.isPressed) 
        {
            jumpPressed = true;
            jumpReleased = false;
        }
        else //button is released
        {
            jumpPressed = false;
            jumpReleased = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
    }
}
