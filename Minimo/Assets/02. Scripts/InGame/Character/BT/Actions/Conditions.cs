public class IsAssignedToBuildingConditionNode : ConditionNode
{
    public IsAssignedToBuildingConditionNode(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        var assigned = /* TODO: check assignment status */ false;
        return assigned ? NodeStatus.Success : NodeStatus.Failure;
    }
}

public class IsTooFarFromBuildingConditionNode : ConditionNode
{
    public IsTooFarFromBuildingConditionNode(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        var tooFar = /* TODO: check distance to building */ false;
        return tooFar ? NodeStatus.Success : NodeStatus.Failure;
    }
}

public class IsProductionCompleteConditionNode : ConditionNode
{
    public IsProductionCompleteConditionNode(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        var complete = /* TODO: check production complete */ false;
        return complete ? NodeStatus.Success : NodeStatus.Failure;
    }
}

public class HasRemainingTasksConditionNode : ConditionNode
{
    public HasRemainingTasksConditionNode(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        var hasTasks = /* TODO: check remaining production tasks */ false;
        return hasTasks ? NodeStatus.Success : NodeStatus.Failure;
    }
}