using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class TileAlphaSystem : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Collider2D _areaCollider;

    private HashSet<Vector3Int> _previousOpaqueCells = new();

    private void Start()
    {
        InitializeTilemap();
    }
    
    private void Update()
    {
        if (!_areaCollider.isActiveAndEnabled) return;
        
        UpdateTileAlphas();
    }
    
    private void InitializeTilemap()
    {
        var bounds = _tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (_tilemap.HasTile(pos))
            {
                TweenTileAlpha(pos, 0);
            }
        }
    }

    private void UpdateTileAlphas()
    {
        foreach (var pos in _previousOpaqueCells)
        {
            if (_tilemap.HasTile(pos))
            {
                TweenTileAlpha(pos, 0f);
            }
        }
        
        var currentOpaqueCells = new HashSet<Vector3Int>();
        
        var bounds = _tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (_tilemap.HasTile(pos))
            {
                var tileWorldCenter = _tilemap.GetCellCenterWorld(pos);
                if (_areaCollider.OverlapPoint(tileWorldCenter))
                {
                    TweenTileAlpha(pos, 1);
                    currentOpaqueCells.Add(pos);
                }
            }
        }
        
        _previousOpaqueCells = currentOpaqueCells;
    }
    
    private void TweenTileAlpha(Vector3Int pos, float targetAlpha, float duration = 0.3f)
    {
        if (!_tilemap.HasTile(pos)) return;

        var currentColor = _tilemap.GetColor(pos);
        var startAlpha = currentColor.a;

        DOTween.To(() => startAlpha, x =>
        {
            currentColor.a = x;
            _tilemap.SetColor(pos, currentColor);
        }, targetAlpha, duration);
    }

    public void ActiveTileAlphaSystem(bool active)
    {
        _areaCollider.enabled = active;

        if (active)
        {
            InitializeTilemap();
        }
        else
        {
            var bounds = _tilemap.cellBounds;
            foreach (var pos in bounds.allPositionsWithin)
            {
                if (_tilemap.HasTile(pos))
                {
                    _tilemap.SetColor(pos, Color.white);
                }
            }
        }
    }
}