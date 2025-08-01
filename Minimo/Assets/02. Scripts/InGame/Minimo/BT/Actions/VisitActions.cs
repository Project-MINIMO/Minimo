using UnityEngine;

public class VisitWaitAction : ActionNode
{
    private readonly int _isWait = Animator.StringToHash("IsWait");

    public VisitWaitAction(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        Blackboard.Animator.SetBool(_isWait, true);
      
        return NodeStatus.Running;
    }
}
