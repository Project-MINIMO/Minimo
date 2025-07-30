using UnityEngine;

public class AssignAction : ActionNode
{
    public AssignAction(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        if (Blackboard.TargetBuilding != null 
            && Blackboard.TargetBuilding.AssignedMinimo != null) return NodeStatus.Failure;
        
        Blackboard.TargetBuilding.PlaceMinimo(Blackboard.Agent.Data);
        return NodeStatus.Success;
    }
}