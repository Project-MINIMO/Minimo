using UnityEngine;

public class PlunderAction : ActionNode
{
    private readonly StrayMinimoObject _owner;
    private float _duration;
    private float _startTime;
    private bool _shouldReset = true;

    public PlunderAction(Blackboard blackboard) : base(blackboard)
    {
        _owner = Blackboard.Agent.GetComponent<StrayMinimoObject>();
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
            
            _duration = 2;
            _startTime = Time.time;
            _owner.TryPlunder();
        }

        if (Blackboard.TargetBuilding.CurrentState != ProduceState.Complete) return NodeStatus.Failure;
        
        if (Time.time - _startTime >= _duration)
        {
            _shouldReset = true;
            var (item, amount) = Blackboard.TargetBuilding.PlunderedResult();
            _owner.SuccessPlunder(item, amount);
            Blackboard.TargetBuilding = null;
            return NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}