public class IsAssignedToBuildingCondition : ConditionNode
{
    public IsAssignedToBuildingCondition(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        var assigned = /* TODO: check assignment status */ false;
        return assigned ? NodeStatus.Success : NodeStatus.Failure;
    }
}

public class IsTooFarFromBuildingCondition : ConditionNode
{
    public IsTooFarFromBuildingCondition(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        var tooFar = /* TODO: check distance to building */ false;
        return tooFar ? NodeStatus.Success : NodeStatus.Failure;
    }
}

public class IsProductionCompleteCondition : ConditionNode
{
    public IsProductionCompleteCondition(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        var complete = /* TODO: check production complete */ false;
        return complete ? NodeStatus.Success : NodeStatus.Failure;
    }
}

public class HasRemainingTasksCondition : ConditionNode
{
    public HasRemainingTasksCondition(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        var hasTasks = /* TODO: check remaining production tasks */ false;
        return hasTasks ? NodeStatus.Success : NodeStatus.Failure;
    }
}