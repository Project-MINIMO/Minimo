using UnityEngine;

public class SwimIdleAction : ActionNode
{
    private readonly int _isSwimIdle = Animator.StringToHash("IsSwimIdle");
    
    private float _duration;
    private float _startTime;
    private bool _shouldReset = true;

    public SwimIdleAction(Blackboard blackboard) : base(blackboard) { }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            
            _duration = Random.Range(5f, 15f);
            _startTime = Time.time;
            Blackboard.Animator.SetBool(_isSwimIdle, true);
        }
        
        if (Time.time - _startTime >= _duration)
        {
            _shouldReset = true;
            Blackboard.Animator.SetBool(_isSwimIdle, false);
            return NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}
