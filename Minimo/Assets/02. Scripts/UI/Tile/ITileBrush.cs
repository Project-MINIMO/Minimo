using UnityEngine;
using UnityEngine.Tilemaps;

public interface ITileBrush
{
    bool Apply(Vector3Int cell);
}

public class PaintBrush : ITileBrush
{
    private readonly Tilemap _tilemap;
    private readonly Tilemap _glowMap;
    private CustomTile _selectedTile;

    public PaintBrush(Tilemap tilemap, Tilemap glowMap)
    {
        _tilemap = tilemap;
        _glowMap = glowMap;
    }
    
    public void SetSelectedTile(CustomTile tile)
    {
        _selectedTile = tile;
    }

    public bool Apply(Vector3Int cell)
    {
        if (_selectedTile == null)
        {
            App.Notification(NotifyType.DeselectTile);
            return false;
        }
        
        if (!_selectedTile.CanInstall())
        {
            App.Notification(NotifyType.GoldLack);
            return false;
        }
        
        AccountInfo.Instance.Gold.AddCount(-_selectedTile.Cost);

        _tilemap.SetTile(cell, _selectedTile.Tile);
        _glowMap.SetTile(cell, _selectedTile.Tile);

        return true;
    }
}

public class EraseBrush : ITileBrush
{
    private readonly Tilemap _tilemap;
    private readonly Tilemap _glowMap;
    private readonly Tilemap _installMap;

    public EraseBrush(Tilemap tilemap, Tilemap glowMap, Tilemap installMap)
    {
        _tilemap = tilemap;
        _glowMap = glowMap;
        _installMap = installMap;
    }

    public bool Apply(Vector3Int cell)
    {
        if (_installMap.GetTile(cell) != null)
        {
            App.Notification(NotifyType.CannotEraseTile);
            return false;
        }
        
        _tilemap.SetTile(cell, null);
        _glowMap.SetTile(cell, null);

        return true;
    }
}
