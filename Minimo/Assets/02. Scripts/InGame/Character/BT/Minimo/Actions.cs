using System.Linq;
using UnityEngine;

public class ActiveProductionAction : ActionNode
{
    private readonly int _isWork = Animator.StringToHash("IsWork");
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
            Blackboard.Animator.SetBool(_isWork, true);
        }

        return Blackboard.Building.CurrentState == ProduceState.Produce 
            ? NodeStatus.Running 
            : NodeStatus.Success;
    }
}

public class CompleteProduceAction : ActionNode
{
    private readonly int _isWork = Animator.StringToHash("IsWork");

    public CompleteProduceAction(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        Blackboard.Animator.SetBool(_isWork, false);

        return Blackboard.Building.ActiveTask == null ? NodeStatus.Failure : NodeStatus.Success;
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
        var groundTiles = Blackboard.Building.PositionData.GroundTilePositions;
        var leftmost = groundTiles.OrderBy(p => p.x)
            .ThenByDescending(p => p.y)
            .First();
        var targetOffset = new Vector3Int(leftmost.x - 1, leftmost.y - 1, 0);
        
        var path = _pathManager.GetPath(
            Blackboard.Agent.transform.position, 
            Blackboard.Building.transform.position,
            targetOffset
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

public class FindNearestWorkPositionAction : ActionNode
{
    private readonly PathManager _pathManager;
    private readonly EditManager _editManager;
    
    public FindNearestWorkPositionAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
        _editManager = App.GetManager<EditManager>();
    }

    public override NodeStatus Tick()
    {
        var emptyAdvances = _editManager.ActiveAdvanceds
            .Where(building => building.AssignedMinimo == null)
            .ToList();
        
        if (emptyAdvances.Count == 0) return NodeStatus.Failure;
        
        var agentPos = Blackboard.Agent.transform.position;
        var nearest = emptyAdvances
            .OrderBy(b => Vector3.SqrMagnitude(b.transform.position - agentPos))
            .First();

        Blackboard.TargetBuilding = nearest;
        
        var groundTiles = nearest.PositionData.GroundTilePositions;
        var leftmost = groundTiles.OrderBy(p => p.x)
            .ThenByDescending(p => p.y)
            .First();
        var targetOffset = new Vector3Int(leftmost.x - 1, leftmost.y - 1, 0);
   
        var path = _pathManager.GetPath(
            agentPos, 
            nearest.transform.position, 
            targetOffset
            );

        if (path == null || path.Count == 0) return NodeStatus.Failure;

        Blackboard.Path = path;
        return NodeStatus.Success;
    }
}

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
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private const string TopRight = "Walk_TR";
    private const string TopLeft = "Walk_TL";
    private const string BottomRight = "Walk_BR";
    private const string BottomLeft = "Walk_BL";
    
    private readonly PathManager _pathManager;
    private Vector3 _targetPosition;
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
                
                Blackboard.Agent.transform.position = Vector3.MoveTowards(
                    Blackboard.Agent.transform.position, 
                    _targetPosition, 
                    _speed * Time.deltaTime);
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
            Blackboard.Animator.speed = 1;
            Blackboard.Animator.SetBool(_isWalk, false);
            return NodeStatus.Success;
        }
    }
}

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