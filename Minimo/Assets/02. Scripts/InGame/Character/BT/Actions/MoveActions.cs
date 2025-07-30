using UnityEngine;

public class MoveAction : ActionNode
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private const string TopRight = "Walk_TR";
    private const string TopLeft = "Walk_TL";
    private const string BottomRight = "Walk_BR";
    private const string BottomLeft = "Walk_BL";
    
    private readonly PathManager _pathManager;
    private Vector3 _targetPosition;
    private Vector3 _prevPosition;
    private int _currentIndex;
    private float _speed;
    private bool _shouldReset = true;
    
    public MoveAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
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
            _currentIndex = 0;
            _targetPosition = _pathManager.GetTileWorldPosition(Blackboard.Path[_currentIndex]);
            _prevPosition = Blackboard.Agent.transform.position;
            _speed = Blackboard.Path.Count > 3 ? 0.6f : 0.3f;
            Blackboard.Animator.speed = Mathf.Approximately(_speed, 0.3f) ? 1 : 2;
            Blackboard.Animator.SetBool(_isWalk, true);
        }

        if (Blackboard.AnimatorIsPlaying("LayToStand")) return NodeStatus.Running;
        if (Blackboard.AnimatorIsPlaying("Boast01")) return NodeStatus.Running;
        if (Blackboard.TargetBuilding != null 
            && Blackboard.TargetBuilding.AssignedMinimo != null) return NodeStatus.Failure;
        
        if (_currentIndex < Blackboard.Path.Count)
        {
            if ((_targetPosition - Blackboard.Agent.transform.position).sqrMagnitude > 0f)
            {
                SetAnimationDirection();
                
                Blackboard.Agent.transform.position = Vector3.MoveTowards(
                    Blackboard.Agent.transform.position, 
                    _targetPosition, 
                    _speed * Time.deltaTime);

                if (_prevPosition == Blackboard.Agent.transform.position)
                {
                    Exit();
                    return NodeStatus.Failure;
                }
                _prevPosition = Blackboard.Agent.transform.position;
            }
            else
            {
                if (++_currentIndex < Blackboard.Path.Count)
                {
                    _targetPosition = _pathManager.GetTileWorldPosition(Blackboard.Path[_currentIndex]);
                }
            }
            
            return NodeStatus.Running;
        }
        else
        {
            Exit();
            return NodeStatus.Success;
        }
    }

    private void Exit()
    {
        _shouldReset = true;
        Blackboard.Animator.speed = 1;
        Blackboard.Animator.SetBool(_isWalk, false);
        Blackboard.TargetBuilding = null;
    }
    
    private void SetAnimationDirection()
    {
        var deltaX = _targetPosition.x - Blackboard.Agent.transform.position.x;
        var deltaY = _targetPosition.y - Blackboard.Agent.transform.position.y;

        var trigger = (deltaX, deltaY) switch
        {
            (> 0, > 0) => TopRight,
            (> 0, < 0) => BottomRight,
            (> 0, 0) => BottomRight,

            (< 0, > 0) => TopLeft,
            (< 0, < 0) => BottomLeft,
            (< 0, 0) => BottomLeft,

            (0, > 0) => // 수직 ↑
                Blackboard.AnimatorIsPlaying(BottomLeft) 
                || Blackboard.AnimatorIsPlaying(TopLeft) 
                    ? TopLeft : TopRight,

            (0, < 0) => // 수직 ↓
                Blackboard.AnimatorIsPlaying(BottomLeft) 
                || Blackboard.AnimatorIsPlaying(TopLeft)
                    ? BottomLeft : BottomRight,

            (0, 0) => TopRight,
            _ => TopRight
        };
                
        Blackboard.Animator.SetTrigger(trigger);
    }
}

public class MoveForwardAction : ActionNode
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private const string TopRight = "Walk_TR";
    private const string TopLeft = "Walk_TL";
    private const string BottomRight = "Walk_BR";
    private const string BottomLeft = "Walk_BL";
    
    private Vector3 _targetPosition;
    private Vector3 _prevPosition;
    private float _speed;
    private bool _shouldReset = true;
    
    public MoveForwardAction(Blackboard blackboard) : base(blackboard) { }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            _targetPosition = Blackboard.TargetPosition;
            _prevPosition = Blackboard.Agent.transform.position;
            _speed = 0.6f;
            Blackboard.Animator.speed = 2;
            Blackboard.Animator.SetBool(_isWalk, true);
            
            SetAnimationDirection();
        }
        
        if ((_targetPosition - Blackboard.Agent.transform.position).sqrMagnitude > 0f)
        {
            Blackboard.Agent.transform.position = Vector3.MoveTowards(
                Blackboard.Agent.transform.position, 
                _targetPosition, 
                _speed * Time.deltaTime);

            if (_prevPosition == Blackboard.Agent.transform.position)
            {
                Exit();
                return NodeStatus.Failure;
            }
            _prevPosition = Blackboard.Agent.transform.position;
            
            return NodeStatus.Running;
        }

        Exit();
        return NodeStatus.Success;
    }

    private void Exit()
    {
        _shouldReset = true;
        Blackboard.Animator.speed = 1;
        Blackboard.Animator.SetBool(_isWalk, false);
    }

    private void SetAnimationDirection()
    {
        var deltaX = _targetPosition.x - Blackboard.Agent.transform.position.x;
        var deltaY = _targetPosition.y - Blackboard.Agent.transform.position.y;

        var trigger = (deltaX, deltaY) switch
        {
            (> 0, > 0) => TopRight,
            (> 0, < 0) => BottomRight,
            (> 0, 0) => BottomRight,

            (< 0, > 0) => TopLeft,
            (< 0, < 0) => BottomLeft,
            (< 0, 0) => BottomLeft,

            (0, > 0) => // 수직 ↑
                Blackboard.AnimatorIsPlaying(BottomLeft) 
                || Blackboard.AnimatorIsPlaying(TopLeft) 
                    ? TopLeft : TopRight,

            (0, < 0) => // 수직 ↓
                Blackboard.AnimatorIsPlaying(BottomLeft) 
                || Blackboard.AnimatorIsPlaying(TopLeft)
                    ? BottomLeft : BottomRight,

            (0, 0) => TopRight,
            _ => TopRight
        };
                
        Blackboard.Animator.SetTrigger(trigger);
    }
}