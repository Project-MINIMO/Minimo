using UnityEngine;

public class IsClicked : ConditionNode
{
    private readonly MinimoObject _owner;

    public IsClicked(Blackboard blackboard) : base(blackboard)
    {
        _owner = blackboard.Agent.GetComponent<MinimoObject>();
    }
    
    public override NodeStatus Tick()
    {
        return _owner.IsClicked ? NodeStatus.Success : NodeStatus.Failure;
    }
}

public class SwimIdleAction : ActionNode
{
    private readonly int _isSwimIdle = Animator.StringToHash("IsSwimIdle");
    private readonly MinimoObject _owner;
    
    private bool _shouldReset = true;

    public SwimIdleAction(Blackboard blackboard) : base(blackboard)
    {
        _owner = blackboard.Agent.GetComponent<MinimoObject>();
    }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            
            Blackboard.Animator.SetBool(_isSwimIdle, true);
        }

        if (!_owner.IsClicked)
        {
            _shouldReset = true;
            Blackboard.Animator.SetBool(_isSwimIdle, false);
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }
}
