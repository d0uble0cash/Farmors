using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected bool JumpPressed {get => player.jumpPressed; set => player.jumpPressed = value;}
    protected bool JumpReleased {get => player.jumpReleased; set => player.jumpReleased = value;}

    protected float jumpCutMultiplier => player.jumpCutMultiplier;
    protected bool RunPressed => player.runPressed;
    protected Vector2 MoveInput => player.moveInput;
    public PlayerState (Player player)
    {
        this.player = player;
        this.animator = player.animator;
        this.rb = player.rb;
    }
    public virtual void Enter() {}
    public virtual void Exit() {}
    public virtual void Update() {}
    public virtual void FixedUpdate() {}
}
