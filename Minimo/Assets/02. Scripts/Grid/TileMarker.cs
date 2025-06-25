using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMarker : MonoBehaviour
{
    [SerializeField] private Tilemap _markTilemap;
    [SerializeField] private TileBase _possibleTile;
    [SerializeField] private TileBase _impossibleTile;
    
    private EditManager _editManager;
    private InstallChecker _installChecker;
    
    private void Start()
    {
        _installChecker = GetComponent<InstallChecker>();
        _editManager = App.GetManager<EditManager>();

        _editManager.IsEditing
            .Subscribe((isEditing) =>
            {
                if (isEditing)
                {
                    if (!_editManager.CurrentEditObject) return;
                    SetMarkTiles(_editManager.CurrentEditObject);
                }
                else
                {
                    ClearMarkTiles();
                }
            }).AddTo((gameObject));

        _editManager.CurrentCellPosition
            .DistinctUntilChanged()
            .Subscribe((position) =>
            {
                if(!_editManager.CurrentEditObject)
                {
                    return;
                }

                SetMarkTiles(_editManager.CurrentEditObject);
            }).AddTo(gameObject);
    }
    
    private void ClearMarkTiles()
    {
        _markTilemap.ClearAllTiles();
    }

    private void SetMarkTiles(BuildingObject gridObject)
    {
        ClearMarkTiles();

        Vector3Int baseCell = _markTilemap.WorldToCell(gridObject.transform.position);
        
        foreach (var relativePos in gridObject.PositionData.GroundTilePositions)
        {
            Vector3Int cellPos = baseCell + new Vector3Int(relativePos.x, relativePos.y, 0);
            bool canInstall = _installChecker.CheckCanInstall(cellPos, TileType.Ground);
            _markTilemap.SetTile(cellPos, canInstall ? _possibleTile : _impossibleTile);
        }
        
        foreach (var relativePos in gridObject.PositionData.WaterTilePositions)
        {
            Vector3Int cellPos = baseCell + new Vector3Int(relativePos.x, relativePos.y, 0);
            bool canInstall = _installChecker.CheckCanInstall(cellPos, TileType.Water);
            _markTilemap.SetTile(cellPos, canInstall ? _possibleTile : _impossibleTile);
        }
    }
}
