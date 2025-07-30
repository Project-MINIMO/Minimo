using UnityEngine;

public class ActiveProductionAction : ActionNode
{
    private readonly int _isWork = Animator.StringToHash("IsWork");
    private bool _shouldReset = true;
    
    public ActiveProductionAction(Blackboard blackboard) : base(blackboard) { }
    
    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            Blackboard.Animator.SetBool(_isWork, true);
        }

        return Blackboard.Building.CurrentState == ProduceState.Produce 
            ? NodeStatus.Running 
            : NodeStatus.Success;
    }
}

public class CompleteProduceAction : ActionNode
{
    private readonly int _isWork = Animator.StringToHash("IsWork");

    public CompleteProduceAction(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        Blackboard.Animator.SetBool(_isWork, false);

        return Blackboard.Building.ActiveTask == null ? NodeStatus.Failure : NodeStatus.Success;
    }
}
