using UnityEngine;
using UnityEngine.Tilemaps;

public struct TileBrushResult
{
    public bool Success;
    public TileChange Change; 
    
    public static TileBrushResult Failed => new TileBrushResult { Success = false };
}

public interface ITileBrush
{
    public CustomTile SelectedTile { get; }
    TileBrushResult Apply(Vector3Int cell);
}

public class PaintBrush : ITileBrush
{
    private readonly Tilemap _tilemap;
    private readonly Tilemap _glowMap;
    private readonly Tilemap _installMap;
    public CustomTile SelectedTile { get; private set; }

    public PaintBrush(Tilemap tilemap, Tilemap glowMap, Tilemap installMap)
    {
        _tilemap = tilemap;
        _glowMap = glowMap;
        _installMap = installMap;
    }
    
    public void SetSelectedTile(CustomTile tile)
    {
        SelectedTile = tile;
    }
    
    public TileBrushResult Apply(Vector3Int cell)
    {
        if (SelectedTile == null)
        {
            App.Notification(NotifyType.DeselectTile);
            return TileBrushResult.Failed;
        }

        if (_tilemap.GetTile(cell) == SelectedTile.Tile)
        {
            return TileBrushResult.Failed;
        }
        
        if (_installMap.GetTile(cell) != null && SelectedTile.Type == TileType.Water)
        {
            App.Notification(NotifyType.CannotInstallWaterTile);
            return TileBrushResult.Failed;
        }
        
        if (!SelectedTile.CanInstall)
        {
            App.Notification(NotifyType.GoldLack);
            return TileBrushResult.Failed;
        }
        
        AccountInfo.Instance.Gold.AddCount(-SelectedTile.Cost);

        _tilemap.SetTile(cell, SelectedTile.Tile);
        _glowMap.SetTile(cell, SelectedTile.Tile);

        return new TileBrushResult
        {
            Success = true,
            Change = new TileChange
            {
                ChangeType = TileChangeType.Install,
                TileId = SelectedTile.ID,
                Position = cell
            }
        };
    }
}

public class EraseBrush : ITileBrush
{
    private readonly Tilemap _tilemap;
    private readonly Tilemap _glowMap;
    private readonly Tilemap _installMap;
    public CustomTile SelectedTile { get; private set; }

    public EraseBrush(Tilemap tilemap, Tilemap glowMap, Tilemap installMap)
    {
        _tilemap = tilemap;
        _glowMap = glowMap;
        _installMap = installMap;
    }

    public TileBrushResult Apply(Vector3Int cell)
    {
        if (_tilemap.GetTile(cell) == null)
        {
            return TileBrushResult.Failed;
        }
        
        if (_installMap.GetTile(cell) != null)
        {
            App.Notification(NotifyType.CannotEraseTile);
            return TileBrushResult.Failed;
        }
        
        _tilemap.SetTile(cell, null);
        _glowMap.SetTile(cell, null);

        return new TileBrushResult
        {
            Success = true,
            Change = new TileChange
            {
                ChangeType = TileChangeType.Remove,
                TileId = -1,
                Position = cell
            }
        };
    }
}
