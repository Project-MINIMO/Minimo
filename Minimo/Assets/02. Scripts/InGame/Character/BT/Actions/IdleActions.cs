using UnityEngine;

public class LayDownAction : ActionNode
{
    private readonly int _isLay = Animator.StringToHash("IsLay");
    
    private float _duration;
    private float _startTime;
    private bool _shouldReset = true;

    public LayDownAction(Blackboard blackboard) : base(blackboard) { }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            
            _duration = Random.Range(15f, 20f);
            _startTime = Time.time;
            Blackboard.Animator.SetBool(_isLay, true);
        }
        
        if (Time.time - _startTime >= _duration)
        {
            _shouldReset = true;
            Blackboard.Animator.SetBool(_isLay, false);
            return NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}
