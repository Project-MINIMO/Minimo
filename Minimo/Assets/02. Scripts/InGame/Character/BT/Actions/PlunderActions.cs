using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlunderAction : ActionNode
{
    private readonly int _isPlunder = Animator.StringToHash("IsPlunder");
    
    private float _duration;
    private float _startTime;
    private bool _shouldReset = true;
    
    public PlunderAction(Blackboard blackboard) : base(blackboard) { }

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
            Blackboard.Animator.SetBool(_isPlunder, true);
        }

        if (Blackboard.TargetBuilding.CurrentState != ProduceState.Complete) return NodeStatus.Failure;
        
        if (Time.time - _startTime >= _duration)
        {
            _shouldReset = true;
            Blackboard.Animator.SetBool(_isPlunder, false);
            var (item, amount) = Blackboard.TargetBuilding.PlunderedResult();
            Blackboard.TargetBuilding = null;
            return NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}