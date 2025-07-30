using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathManager : ManagerBase
{
    [SerializeField] private Tilemap _checkTilemap;
    [SerializeField] private InstallChecker _installChecker;
    
    public Vector3 GetTileWorldPosition(Vector3Int tilePosition)
    {
        return _checkTilemap.GetCellCenterWorld(tilePosition);
    }

    public List<Vector3Int> GetRandomPath(Vector3 currentPosition, int searchRadius = 10)
    {
        var currentCell = _checkTilemap.WorldToCell(currentPosition);
        var targetCell = GetRandomWalkableTile(currentCell, searchRadius);
        
        return FindPath(currentCell, targetCell);
    }

    public List<Vector3Int> GetPath(Vector3 currentPosition, Vector3 targetPosition, Vector3Int offset)
    {
        var currentCell = _checkTilemap.WorldToCell(currentPosition);
        var targetPos = targetPosition + GetTileWorldPosition(offset);
        var targetCell = _checkTilemap.WorldToCell(targetPos);
        return FindPath(currentCell, targetCell);
    }

    private Vector3Int GetRandomWalkableTile(Vector3Int center, int size)
    {
        List<Vector3Int> walkableTiles = new();

        for (var x = center.x - size / 2; x <= center.x + size / 2; x++)
        {
            for (var y = center.y - size / 2; y <= center.y + size / 2; y++)
            {
                Vector3Int position = new(x, y, 0);
                if (IsWalkable(position))
                {
                    walkableTiles.Add(position);
                }
            }
        }

        if (walkableTiles.Count > 0)
        {
            var randomIndex = Random.Range(0, walkableTiles.Count);
            return walkableTiles[randomIndex];
        }

        Debug.LogWarning("No walkable positions found.");
        return Vector3Int.zero;
    }
    
    private Vector3Int GetRandomTile(Vector3Int center, int size)
    {
        var half = size / 2;
        var randomX = Random.Range(center.x - half, center.x + half + 1);
        var randomY = Random.Range(center.y - half, center.y + half + 1);
        
        return new Vector3Int(randomX, randomY, center.z);
    }
    
    private bool IsWalkable(Vector3Int position) => _installChecker.CheckCanInstall(position);

    #region A* Algorithm
    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int target)
    {
        PriorityQueue<AStarNode> openList = new();
        HashSet<Vector3Int> closedSet = new();

        AStarNode startNode = new(start, null, 0, GetHeuristic(start, target));
        openList.Enqueue(startNode, startNode.F);
        
        var maxIterations = 10000; 
        var iterations = 0;

        while (openList.Count > 0)
        {
            if (iterations++ > maxIterations)
            {
                Debug.LogError("FindPath: Too many iterations, exiting to prevent infinite loop.");
                return null;
            }
            
            var currentNode = openList.Dequeue();

            if (currentNode.Position == target)
            {
                return ConstructPath(currentNode);
            }

            closedSet.Add(currentNode.Position);

            foreach (var neighborPos in GetNeighbors(currentNode.Position))
            {
                if (closedSet.Contains(neighborPos) || !IsWalkable(neighborPos))
                {
                    continue;
                }
                
                if (!IsWalkable(neighborPos)) continue;

                var tentativeGScore = currentNode.G + GetDistance(currentNode.Position, neighborPos);

                var existingNode = openList.Find(n => n.Position == neighborPos);

                if (existingNode == null || tentativeGScore < existingNode.G)
                {
                    AStarNode neighborNode = new(neighborPos, currentNode, tentativeGScore, GetHeuristic(neighborPos, target));
                    openList.Enqueue(neighborNode, neighborNode.F);
                }
            }
        }

        return null; // Path not found
    }

    private float GetHeuristic(Vector3Int a, Vector3Int b) 
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance

    private float GetDistance(Vector3Int a, Vector3Int b)
        => Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2)); // Euclidean distance for isometric

    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int position)
    {
        yield return new Vector3Int(position.x + 1, position.y, position.z);
        yield return new Vector3Int(position.x - 1, position.y, position.z);
        yield return new Vector3Int(position.x, position.y + 1, position.z);
        yield return new Vector3Int(position.x, position.y - 1, position.z);

        yield return new Vector3Int(position.x + 1, position.y + 1, position.z);
        yield return new Vector3Int(position.x + 1, position.y - 1, position.z);
        yield return new Vector3Int(position.x - 1, position.y + 1, position.z);
        yield return new Vector3Int(position.x - 1, position.y - 1, position.z);
    }
    
    private List<Vector3Int> ConstructPath(AStarNode node)
    {
        List<Vector3Int> path = new();

        while (node != null)
        {
            path.Add(node.Position);
            node = node.Parent;
        }

        path.Reverse();
        return path;
    }
    #endregion
}