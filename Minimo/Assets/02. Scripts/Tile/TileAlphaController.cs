using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAlphaController : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Collider2D _areaCollider;

    private readonly Color _transparentColor = new(1f, 1f, 1f, 0f);
    private readonly Color _opaqueColor = new(1f, 1f, 1f, 1f);
    
    private HashSet<Vector3Int> _previousOpaqueCells = new();

    private void Start()
    {
        InitializeTilemap();
    }
    
    private void InitializeTilemap()
    {
        var bounds = _tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (_tilemap.HasTile(pos))
            {
                _tilemap.SetColor(pos, _transparentColor);
            }
        }
    }
    
    private void Update()
    {
        UpdateTileAlphas();
    }

    private void UpdateTileAlphas()
    {
        var bounds = _tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (_tilemap.HasTile(pos))
            {
                _tilemap.SetColor(pos, _transparentColor);
            }
        }
        
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (_tilemap.HasTile(pos))
            {
                var tileWorldCenter = _tilemap.GetCellCenterWorld(pos);
                if (_areaCollider.OverlapPoint(tileWorldCenter))
                {
                    _tilemap.SetColor(pos, _opaqueColor);
                }
            }
        }
    }
}