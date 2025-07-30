public class AssignAction : ActionNode
{
    private readonly MinimoObject _owner;
    
    public AssignAction(Blackboard blackboard) : base(blackboard)
    {
        _owner = blackboard.Agent.GetComponent<MinimoObject>();
    }

    public override NodeStatus Tick()
    {
        if (Blackboard.TargetBuilding != null 
            && Blackboard.TargetBuilding.AssignedMinimo != null) return NodeStatus.Failure;
        
        Blackboard.TargetBuilding.PlaceMinimo(_owner.Data);
        return NodeStatus.Success;
    }
}