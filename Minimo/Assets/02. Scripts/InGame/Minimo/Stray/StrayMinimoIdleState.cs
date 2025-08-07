using UnityEngine;

public class StrayMinimoIdleState : State<StrayMinimoObject>
{
    private readonly int Idle = Animator.StringToHash("Idle");

    public StrayMinimoIdleState(StrayMinimoObject owner) : base(owner) { }

    public override void Enter()
    {
        Animator.SetBool(Idle, true);
    }

    public override void Execute() { }

    public override void Exit()
    {
        Animator.SetBool(Idle, false);
    }
}
