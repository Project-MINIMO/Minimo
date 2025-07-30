using System.Linq;
using UnityEngine;

public class FindWorkPositionAction : ActionNode
{
    private ProduceAdvanced Building => _owner.Data.AssignedBuilding;
    private readonly MinimoObject _owner;
    private readonly PathManager _pathManager;
    
    public FindWorkPositionAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
        _owner = blackboard.Agent.GetComponent<MinimoObject>();
    }

    public override NodeStatus Tick()
    {
        var groundTiles = Building.PositionData.GroundTilePositions;
        var leftmost = groundTiles.OrderBy(p => p.x)
            .ThenByDescending(p => p.y)
            .First();
        var targetOffset = new Vector3Int(leftmost.x - 1, leftmost.y - 1, 0);
        
        var path = _pathManager.GetPath(
            Blackboard.Agent.transform.position, 
            Building.transform.position,
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

public class FindStrayPositionAction  : ActionNode
{
    public FindStrayPositionAction(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        var currentPos = Blackboard.Agent.transform.position;
        var randomOffset2D = Random.insideUnitCircle * 5f;
        var randomPos = new Vector3(
            currentPos.x + randomOffset2D.x,
            currentPos.y + randomOffset2D.y,
            currentPos.z);
        
        Blackboard.TargetPosition = randomPos;

        return NodeStatus.Success;
    }
}

public class FindCompletePositionAction : ActionNode
{
    private readonly EditManager _editManager;
    
    public FindCompletePositionAction(Blackboard blackboard) : base(blackboard)
    {
        _editManager = App.GetManager<EditManager>();
    }

    public override NodeStatus Tick()
    {
        var emptyAdvances = _editManager.ActiveAdvanceds
            .Where(building => building.CurrentState == ProduceState.Complete)
            .ToList();
        
        if (emptyAdvances.Count == 0) return NodeStatus.Failure;
        
        var agentPos = Blackboard.Agent.transform.position;
        var nearest = emptyAdvances
            .OrderBy(b => Vector3.SqrMagnitude(b.transform.position - agentPos))
            .First();

        Blackboard.TargetBuilding = nearest;
        Blackboard.TargetPosition = nearest.transform.position;
        
        return NodeStatus.Success;
    }
}

public class FindNearestCornerPositionAction : ActionNode
{
    private readonly CameraBoundsUpdater _mapBounds;

    public FindNearestCornerPositionAction(Blackboard blackboard) : base(blackboard)
    {
        _mapBounds = GameObject.FindWithTag("MapBounds").GetComponent<CameraBoundsUpdater>();
    }

    public override NodeStatus Tick()
    {
        var agentPos = Blackboard.Agent.transform.position;
        var nearest = _mapBounds.GetCorners()
            .OrderBy(corner => Vector3.SqrMagnitude(corner - agentPos))
            .First();

        Blackboard.TargetPosition = nearest;
        
        return NodeStatus.Success;
    }
}


