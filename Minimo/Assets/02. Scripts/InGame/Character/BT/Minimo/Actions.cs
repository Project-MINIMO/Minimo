using System.Linq;
using UnityEngine;

public class ActiveProductionAction : ActionNode
{
    private static readonly int IsWork = Animator.StringToHash("IsWork");
    private bool _shouldReset = true;
    
    public ActiveProductionAction(Blackboard blackboard) : base(blackboard) { }
    
    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            Blackboard.Animator.SetBool(IsWork, true);
        }

        return Blackboard.Building.CurrentState == ProduceState.Produce 
            ? NodeStatus.Running 
            : NodeStatus.Success;
    }
}

public class CompleteProduceAction : ActionNode
{
    private static readonly int Complete = Animator.StringToHash("Complete");
    private static readonly int IsWork = Animator.StringToHash("IsWork");
    private bool _shouldReset = true;
    
    private float _startTime;
    
    public CompleteProduceAction(Blackboard blackboard) : base(blackboard) { }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            _startTime = Time.time;
            
            Blackboard.Animator.SetTrigger(Complete);
        }
       
        if (Time.time - _startTime >= 2.1f)
        {
            _shouldReset = true;
            Blackboard.Animator.SetBool(IsWork, false);
            return Blackboard.Building.ActiveTask == null ? NodeStatus.Failure : NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}

public class FindWorkPositionAction : ActionNode
{
    private readonly PathManager _pathManager;
    
    public FindWorkPositionAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
    }

    public override NodeStatus Tick()
    {
        var path = _pathManager.GetPath(
            Blackboard.Agent.transform.position, 
            Blackboard.Building.transform.position
            );

        if (path is { Count: > 0 })
        {
            Blackboard.Path = path;
            return NodeStatus.Success;
        }
        else
        {
            return NodeStatus.Failure;
        }
    }
}

public class FindRestPositionAction : ActionNode
{
    private readonly PathManager _pathManager;
    
    public FindRestPositionAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
    }

    public override NodeStatus Tick()
    {
        var path = _pathManager.GetRandomPath(Blackboard.Agent.transform.position);

        if (path is { Count: > 0 })
        {
            Blackboard.Path = path;
            return NodeStatus.Success;
        }
        else
        {
            return NodeStatus.Failure;
        }
    }
}

public class MoveAction : ActionNode
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");
    private const string TopRight = "TR";
    private const string TopLeft = "TL";
    private const string BottomRight = "BR";
    private const string BottomLeft = "BL";
    
    private const float Speed = 0.3f;
    
    private readonly PathManager _pathManager;
    private Vector3 _targetPosition;
    private int _currentIndex;
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
            Blackboard.Animator.SetBool(IsWalk, true);
        }
        
        if (_currentIndex < Blackboard.Path.Count)
        {
            if ((_targetPosition - Blackboard.Agent.transform.position).sqrMagnitude > 0.05f)
            {
                var deltaX = _targetPosition.x - Blackboard.Agent.transform.position.x;
                var deltaY = _targetPosition.y - Blackboard.Agent.transform.position.y;

                var trigger = deltaX switch
                {
                    > 0 when deltaY > 0 => TopRight,
                    < 0 when deltaY > 0 => TopLeft,
                    < 0 when deltaY < 0 => BottomLeft,
                    > 0 when deltaY < 0 => BottomRight,
                    _ => TopRight
                };
                
                Blackboard.Agent.SetAnimation(trigger);
                
                Blackboard.Agent.transform.position = Vector3.MoveTowards(
                    Blackboard.Agent.transform.position, 
                    _targetPosition, 
                    Speed * Time.deltaTime);
                
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
            _shouldReset = true;
            Blackboard.Animator.SetBool(IsWalk, false);
            return NodeStatus.Success;
        }
    }
}

public class LayDownAction : ActionNode
{
    private static readonly int IsLay = Animator.StringToHash("IsLay");
    
    private float _duration;
    private float _startTime;
    private bool _shouldReset = true;
    private bool _isLayEnd = false;

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
            _isLayEnd = false;
            
            _duration = Random.Range(15f, 20f);
            _startTime = Time.time;
            Blackboard.Animator.SetBool(IsLay, true);
        }

        if (Time.time - _startTime >= _duration + 2.1f)
        {
            _shouldReset = true;
            return NodeStatus.Success;
        }
        
        if (!_isLayEnd && Time.time - _startTime >= _duration)
        {
            _isLayEnd = true;
            Blackboard.Animator.SetBool(IsLay, false);
            return NodeStatus.Running;
        }
        
        return NodeStatus.Running;
    }
}