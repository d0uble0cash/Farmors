using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
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
    private  bool slideInputLocked;
    private float slideTimer;
    private float slideStopTimer;

   [Header("Coyote Time")]
   public float coyoteTimeDuration = 0.12f;

   [Header("Jump Buffer")]
   public float jumpBufferDuration = 0.12f;
   
   //Inputs
   private Vector2 moveInput;
   private bool runPressed;
   private bool jumpPressed;
   private bool jumpReleased;

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
    }


    private void Start()
   {
        if (rb != null) rb.gravityScale = normalGravity;
   }


   void Update()
    {
        if (!isSliding)
        {
            Flip();
        }
        HandleAnimations();
        HandleSlide();
        TryStandUp();
        TickTimers();
    }

   void FixedUpdate()
    {
        CheckGrounded();
        if (currentMode == GameMode.Platformer)
        {   
            ApplyVariableGravity();
            if (!isSliding)
            {
                HandleMovement();
            }
            HandleJump();
        } else
        {
            rb.gravityScale = 0f;
            HandleTopDownMovement();
        }
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

    private void HandleJump()
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
    }

    private void HandleSlide()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(slideSpeed * facingDirection, rb.linearVelocity.y);
            //If we are done sliding
            if (slideTimer <= 0)
            {
                isSliding = false;
                slideStopTimer = slideStopDuration;
                TryStandUp();
            }
        }

        if (slideStopTimer > 0)
        {
            slideStopTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        //Start the slide
        if (isGrounded && runPressed && moveInput.y < -.1f && !isSliding && !slideInputLocked)
        {
            isSliding = true;
            slideInputLocked = true;
            slideTimer = slideDuration;
            SetColliderSlide();
        }

        if (slideStopTimer < 0 && moveInput.y >= -.1f)
        {
            slideInputLocked = false;
        }
    }

    void SetColliderNormal()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, normalHeight);
        playerCollider.offset = normalOffset;
    }

    void SetColliderSlide()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        playerCollider.offset = slideOffset;
    }

    void TryStandUp()
    {
        if (headCheck == null) return;
        if (isSliding) return;
        
        bool ceilingAbove = Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
        bool pressingDown = moveInput.y < -.1f;
        if (ceilingAbove || pressingDown)
        {
            SetColliderSlide();
            animator.SetBool("isCrouching", true);
        } else
        {
            SetColliderNormal();
            animator.SetBool("isCrouching", false);  
        }
    }

    private void HandleMovement()
    {
        if (rb == null) return; //null check
        float currentSpeed = runPressed ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput.x * currentSpeed, rb.linearVelocity.y);
    }

    private void ApplyVariableGravity()
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

    void HandleAnimations()
    {
        //FIX : null check so missing Animator doesn't crash the game
        if (animator == null) return;
        bool isCrouching = animator.GetBool("isCrouching");
        if(currentMode == GameMode.Platformer) {
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isJumping", rb.linearVelocity.y > 0.1f);
            animator.SetBool("isSliding", isSliding);
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
            
            bool isMoving = Mathf.Abs(moveInput.x) > .1f && isGrounded;
            animator.SetBool("isIdle", !isMoving && isGrounded && !isSliding && !isCrouching);
            animator.SetBool("isWalking", isMoving && !runPressed && !isSliding && !isCrouching);   
            animator.SetBool("isRunning", isMoving && runPressed && !isSliding && !isCrouching); 
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
