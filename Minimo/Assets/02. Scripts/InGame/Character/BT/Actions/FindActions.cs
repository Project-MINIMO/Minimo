using System.Linq;
using UnityEngine;

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
