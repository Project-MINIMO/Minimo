using UnityEngine;

public class MoveToBuildingActionNode : ActionNode
{
    public MoveToBuildingActionNode(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        // TODO: implement movement towards building
        bool arrived = /* movement logic */ false;
        if (!arrived)
            return NodeStatus.Running;

        return NodeStatus.Success;
    }
}

public class PlaceMinimoActionNode : ActionNode
{
    public PlaceMinimoActionNode(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        // TODO: implement placement logic
        return NodeStatus.Success;
    }
}

public class PlayProductionAnimationActionNode : ActionNode
{
    public PlayProductionAnimationActionNode(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        // TODO: trigger production animation
        bool animationDone = /* check animation state */ true;
        return animationDone ? NodeStatus.Success : NodeStatus.Running;
    }
}

public class PlayCompletionAnimationActionNode : ActionNode
{
    public PlayCompletionAnimationActionNode(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        // TODO: trigger completion animation
        bool animationDone = /* check animation state */ true;
        return animationDone ? NodeStatus.Success : NodeStatus.Running;
    }
}

public class PlayIdleAnimationActionNode : ActionNode
{
    public PlayIdleAnimationActionNode(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        // TODO: trigger idle animation
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
    private const float Speed = 0.3f;
    
    private readonly PathManager _pathManager;
    private Vector3 _targetPosition;
    private int _currentIndex;
    private bool _shouldReset = true;
    
    private const string TopRight = "TR";
    private const string TopLeft = "TL";
    private const string BottomRight = "BR";
    private const string BottomLeft = "BL";
    
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
            Debug.Log("MoveAction");
            _shouldReset = false;
            _currentIndex = 0;
            _targetPosition = _pathManager.GetTileWorldPosition(Blackboard.Path[_currentIndex]);
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
            return NodeStatus.Success;
        }
    }
}

public class LayDownAction : ActionNode
{
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
        }

        if (Time.time - _startTime >= _duration)
        {
            _shouldReset = true;
            Debug.Log("LayDownAction");
            return NodeStatus.Success;
        }
        else
        {
            return NodeStatus.Running;
        }
    }
}