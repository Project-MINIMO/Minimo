using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

#region User
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
            .ThenBy(p => p.y)
            .First();
        
        var path = _pathManager.GetPath(
            Blackboard.Agent.transform.position, 
            Building.transform.position,
            (Vector3Int)leftmost
        );

        if (path is { Count: > 0 })
        {
            Blackboard.Path = path;
            Blackboard.Speed = 2;
        }
        else
        {
            var targetPos = _pathManager.GetTileWorldPosition(Building.transform.position, (Vector3Int)leftmost);
            Blackboard.Path = new()
            {
                targetPos.Item2
            };
            Blackboard.Agent.transform.position = targetPos.Item1;

        }
        
        return NodeStatus.Success;
    }
}
#endregion

#region Stray
public class FindStrayPositionAction  : ActionNode
{
    private readonly Tilemap _groundTilemap;
    
    private const float MarginMin = 0.5f;
    private const float MarginMax = 1f;  

    public FindStrayPositionAction(Blackboard blackboard) : base(blackboard)
    {
        _groundTilemap = GameObject.FindWithTag("VillageTilemap").GetComponent<Tilemap>();
    }

    public override NodeStatus Tick()
    {
        var cb = _groundTilemap.cellBounds;
        var min = _groundTilemap.GetCellCenterWorld(cb.min);
        var max  = _groundTilemap.GetCellCenterWorld(new Vector3Int(cb.max.x - 1, cb.max.y - 1, cb.max.z));
        var center = (min + max) * 0.5f;
        var radiusX = (max.x - min.x) * 0.5f;
        var radiusY = (max.y - min.y) * 0.5f;
        
        var pos2D = Blackboard.Agent.transform.position;
        
        var dx = (pos2D.x - center.x) / radiusX;
        var dy = (pos2D.y - center.y) / radiusY;
        var isInside = dx * dx + dy * dy <= 1f;
        
        var angle = Random.Range(0f, Mathf.PI * 2f);
        var dir  = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

        var boundaryPoint = center + new Vector3(dir.x * radiusX, dir.y * radiusY, 0);

        var offset = Random.Range(MarginMin, MarginMax);
        var target2D = isInside
            ? boundaryPoint + dir * offset
            : boundaryPoint - dir * offset;
        
        Blackboard.TargetPosition = new Vector3(target2D.x, target2D.y,
            Blackboard.Agent.transform.position.z);

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
        var emptyAdvances = _editManager.ActiveProduces
            .Where(building => building.CurrentState == ProduceState.Complete)
            .ToList();
        
        if (emptyAdvances.Count == 0) return NodeStatus.Failure;
        
        var agentPos = Blackboard.Agent.transform.position;
        var nearest = emptyAdvances
            .OrderBy(b => Vector3.SqrMagnitude(b.transform.position - agentPos))
            .First();

        Blackboard.TargetBuilding = nearest;
        Blackboard.TargetPosition = nearest.transform.position + Vector3.up;
        Blackboard.Speed = 4;
        
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
        Blackboard.Speed = 5;
        
        return NodeStatus.Success;
    }
}
#endregion

#region Visit
public class FindVisitTargetPositionAction : ActionNode
{
    private ProduceObject Building => _owner.Target;
    private readonly VisitMinimoObject _owner;
    private readonly PathManager _pathManager;

    public FindVisitTargetPositionAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
        _owner = blackboard.Agent.GetComponent<VisitMinimoObject>();
    }

    public override NodeStatus Tick()
    {
        var groundTiles = Building.PositionData.GroundTilePositions;
        var leftmost = groundTiles.OrderBy(p => p.x)
            .ThenByDescending(p => p.y)
            .First();
        
        var path = _pathManager.GetPath(
            Blackboard.Agent.transform.position, 
            Building.transform.position,
            (Vector3Int)leftmost
        );

        if (path is { Count: > 0 })
        {
            Blackboard.Path = path;
            Blackboard.Speed = 2;
        }
        else
        {
            var targetPos = _pathManager.GetTileWorldPosition(Building.transform.position, (Vector3Int)leftmost);
            Blackboard.Path = new()
            {
                targetPos.Item2
            };
            Blackboard.Agent.transform.position = targetPos.Item1;

        }
        
        return NodeStatus.Success;
    }
}

public class FindSpaceshipPositionAction : ActionNode
{
    private readonly PathManager _pathManager;

    public FindSpaceshipPositionAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
    }

    public override NodeStatus Tick()
    {
        var path = _pathManager.GetPath(
            Blackboard.Agent.transform.position, 
            new Vector3(1, 1.3f, 0)
        );

        if (path is { Count: > 0 })
        {
            Blackboard.Path = path;
            Blackboard.Speed = 2;
        }
        else
        {
            var targetPos = _pathManager.GetTileWorldPosition(new Vector3(1, 1.3f, 0));
            Blackboard.Path = new()
            {
                targetPos.Item2
            };
            Blackboard.Agent.transform.position = targetPos.Item1;
        }
        
        return NodeStatus.Success;
    }
}
#endregion
