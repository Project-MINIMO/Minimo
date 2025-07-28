using UnityEngine;
using UnityEngine.Tilemaps;

public class TileStateModifier : MonoBehaviour
{
    [SerializeField] private Tilemap _installTilemap;
    [SerializeField] private TileBase _installedTile;

    public void ModifyTileState(BuildingObject gridObject, TileState tileState)
    {
        var baseCell = _installTilemap.WorldToCell(gridObject.transform.position);
        
        foreach (var relativePos in gridObject.PositionData.GroundTilePositions)
        {
            var cellPos = baseCell + new Vector3Int(relativePos.x, relativePos.y, 0);
            _installTilemap.SetTile(cellPos, tileState == TileState.Installed ? _installedTile : null);
        }
        
        foreach (var relativePos in gridObject.PositionData.WaterTilePositions)
        {
            var cellPos = baseCell + new Vector3Int(relativePos.x, relativePos.y, 0);
            _installTilemap.SetTile(cellPos, tileState == TileState.Installed ? _installedTile : null);
        }
    }
}
