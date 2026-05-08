using UnityEngine;

public abstract class BossState
{
    protected StagBoss boss;
    protected Rigidbody2D rb;
    protected Animator anim;
    protected BossStateMachine stateMachine;
 
    protected virtual string AnimBoolName => null;
 
    protected BossState(StagBoss boss)
    {
        this.boss     = boss;
        this.rb       = boss.RB;
        this.anim     = boss.anim;
        this.stateMachine = boss.StateMachine;
    }
 
    public virtual void Enter()
    {
        if (!string.IsNullOrEmpty(AnimBoolName))
            anim?.SetBool(AnimBoolName, true);
    }
 
    public virtual void Update()   { }
    public virtual void FixedUpdate() { }
 
    public virtual void Exit()
    {
        if (!string.IsNullOrEmpty(AnimBoolName))
            anim?.SetBool(AnimBoolName, false);
    }
}
