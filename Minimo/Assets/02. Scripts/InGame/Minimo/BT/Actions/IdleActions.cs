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

public class SitAction : ActionNode
{
    private readonly int _randomIndex = Animator.StringToHash("RandomIndex");
    private readonly int _isSit = Animator.StringToHash("IsSit");
    private readonly MinimoObject _minimo;
    
    private float _duration;
    private float _startTime;
    private float _lastTime;
    private bool _shouldReset = true;

    public SitAction(Blackboard blackboard) : base(blackboard)
    {
        _minimo = blackboard.Agent.GetComponent<MinimoObject>();
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
            
            _duration = Random.Range(15f, 30f);
            _startTime = Time.time;
            _lastTime = _startTime;
            Blackboard.Animator.SetInteger(_randomIndex, Random.Range(0, 2));
            Blackboard.Animator.SetBool(_isSit, true);
        }

        if (Time.time - _lastTime >= 1f && _minimo.Energy < _minimo.MaxEnergy)
        {
            _lastTime = Time.time;
            _minimo.Energy += _minimo.RestEnergy;
        }
        
        if (Time.time - _startTime >= _duration)
        {
            _shouldReset = true;
            Blackboard.Animator.SetBool(_isSit, false);
            return NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}
