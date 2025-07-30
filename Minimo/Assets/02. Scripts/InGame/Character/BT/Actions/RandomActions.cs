using UnityEngine;

public class GetRandomActionIndex : ActionNode
{
    private readonly int _randomMax;

    public GetRandomActionIndex(Blackboard blackboard, int randomMax) : base(blackboard)
    {
        _randomMax = randomMax;
    }
    public override NodeStatus Tick()
    {
        Blackboard.RandomActionIndex = Random.Range(0, _randomMax);
        
        return NodeStatus.Failure;
    }
}

public class IsEqualIndex : ConditionNode
{
    private readonly int _index;

    public IsEqualIndex(Blackboard blackboard, int index) : base(blackboard)
    {
        _index = index;
    }
    
    public override NodeStatus Tick()
    {
        var isEqual = _index == Blackboard.RandomActionIndex;
        return isEqual ? NodeStatus.Success : NodeStatus.Failure;
    }
}
