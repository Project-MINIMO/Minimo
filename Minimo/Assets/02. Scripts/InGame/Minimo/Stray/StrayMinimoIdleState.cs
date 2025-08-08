using UnityEngine;

public class StrayMinimoIdleState : State<StrayMinimoObject>
{
    private readonly int IsPlunder = Animator.StringToHash("IsPlunder");

    public StrayMinimoIdleState(StrayMinimoObject owner) : base(owner) { }

    public override void Enter()
    {
        Animator.SetBool(IsPlunder, false);
    }

    public override void Execute() { }

    public override void Exit()
    {
        Animator.SetBool(IsPlunder, true);
    }
}
