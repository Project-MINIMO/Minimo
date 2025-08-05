using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class TileHighlighter : MonoBehaviour
{
    [SerializeField] private Tilemap _highlightTilemap;
    [SerializeField] private TileBase _highlightTile;
    
    public void SetHighlight(BuildingObject gridObject)
    {
        _highlightTilemap.ClearAllTiles();
        
        var baseCell = _highlightTilemap.WorldToCell(gridObject.transform.position);
 
        foreach (var relativePos in gridObject.PositionData.GroundTilePositions)
        {
            var cellPos = baseCell + new Vector3Int(relativePos.x, relativePos.y, 0);
            _highlightTilemap.SetTile(cellPos, _highlightTile);
        }
        
        foreach (var relativePos in gridObject.PositionData.WaterTilePositions)
        {
            var cellPos = baseCell + new Vector3Int(relativePos.x, relativePos.y, 0);
            _highlightTilemap.SetTile(cellPos, _highlightTile);
        }

        Blink();
    }

    public void ClearHighlight()
    {
        _highlightTilemap.ClearAllTiles();
        _highlightTilemap.DOKill(); 
    }
    
    private void Blink(float minAlpha = 0.3f, float maxAlpha = 1f, float duration = 0.5f)
    {
        _highlightTilemap.DOKill(); 
        
        var color = _highlightTilemap.color;
        color.a = minAlpha;
        _highlightTilemap.color = color;

        DOTween.To(
                () => _highlightTilemap.color.a,
                alpha =>
                {
                    var currentColor = _highlightTilemap.color;
                    currentColor.a = alpha;
                    _highlightTilemap.color = currentColor;
                },
                maxAlpha,
                duration
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetId(_highlightTilemap);
    }
}
