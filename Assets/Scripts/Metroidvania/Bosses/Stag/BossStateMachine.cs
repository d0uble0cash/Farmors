using UnityEngine;

public class BossStateMachine
{
    public BossState CurrentState { get; private set; }
 
    public void Initialize(BossState startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }
 
    public void ChangeState(BossState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
