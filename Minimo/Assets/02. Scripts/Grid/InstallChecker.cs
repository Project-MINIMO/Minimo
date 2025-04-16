using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    Ground,
    Water
}

public class InstallChecker : MonoBehaviour
{
    [SerializeField] private Tilemap _checkTilemap;
    [SerializeField] private Tilemap _installTilemap;
    
    [SerializeField] private TileBase _groundTile;
    [SerializeField] private TileBase _waterTile;
    
    public bool CheckCanInstall(BuildingObject gridObject)
    {
        if (gridObject == null || gridObject.BuildingData == null || gridObject.PositionData == null)
        {
            return false;
        }
        
        var baseCell = _installTilemap.WorldToCell(gridObject.transform.position);
        foreach (var tile in gridObject.PositionData.GroundTilePositions)
        {
            var cellPos = baseCell + new Vector3Int(tile.x, tile.y, 0);
            var checkTile = _checkTilemap.GetTile(cellPos);
            var installTile = _installTilemap.GetTile(cellPos);
            if (checkTile != _groundTile || installTile != null)
            {
                return false;
            }
        }
        foreach (var tile in gridObject.PositionData.WaterTilePositions)
        {
            var cellPos = baseCell + new Vector3Int(tile.x, tile.y, 0);
            var checkTile = _checkTilemap.GetTile(cellPos);
            var installTile = _installTilemap.GetTile(cellPos);
            if (checkTile != _waterTile || installTile != null)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckCanInstall(Vector3Int position)
    {
        var checkTile = _checkTilemap.GetTile(position);
        var installTile = _installTilemap.GetTile(position);
        
        return checkTile == _groundTile && installTile == null;
    }
    
    public bool CheckCanInstall(Vector3Int position, TileType tileType)
    {
        var checkTile = _checkTilemap.GetTile(position);
        var installTile = _installTilemap.GetTile(position);
        
        return checkTile == GetCheckTile(tileType) && installTile == null;
    }
    
    private TileBase GetCheckTile(TileType tileType) => tileType switch
    {
        TileType.Ground => _groundTile,
        TileType.Water => _waterTile,
        _ => null
    };
    
    public List<Vector3> GetInstallablePositions()
    {
        var positions = new List<Vector3>();
        
        foreach (var position in _installTilemap.cellBounds.allPositionsWithin)
        {
            if (CheckCanInstall(position))
            {
                var worldPosition = _installTilemap.CellToWorld(position);
                positions.Add(worldPosition);
            }
        }

        return positions;
    }
}
