using UnityEngine;

public class ActiveProductionAction : ActionNode
{
    private readonly int _isWork = Animator.StringToHash("IsWork");
    
    private ProduceAdvanced Building => _owner.Data.AssignedBuilding;
    private readonly MinimoObject _owner;
    private bool _shouldReset = true;

    public ActiveProductionAction(Blackboard blackboard) : base(blackboard)
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
            Blackboard.Animator.SetBool(_isWork, true);
        }

        return Building.CurrentState == ProduceState.Produce 
            ? NodeStatus.Running 
            : NodeStatus.Success;
    }
}

public class CompleteProduceAction : ActionNode
{
    private readonly int _randomIndex = Animator.StringToHash("RandomIndex");
    private readonly int _isWork = Animator.StringToHash("IsWork");
    
    private ProduceAdvanced Building => _owner.Data.AssignedBuilding;
    private readonly MinimoObject _owner;

    public CompleteProduceAction(Blackboard blackboard) : base(blackboard)
    {
        _owner = blackboard.Agent.GetComponent<MinimoObject>();
    }
    
    public override NodeStatus Tick()
    {
        Blackboard.Animator.SetInteger(_randomIndex, Random.Range(0, 2));
        Blackboard.Animator.SetBool(_isWork, false);

        return NodeStatus.Success;
    }
}
