using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile 
{
    public bool CanInstall => AccountInfo.Instance.Gold.Count >= Cost;
    
    public readonly int ID;
    public readonly TileType Type;
    public readonly TileBase Tile;
    public readonly Sprite Icon;
    public readonly int UnlockLevel;
    public readonly int Cost;

    public bool IsLocked => AccountInfo.Instance.Level.Count < UnlockLevel;
    
    public CustomTile(CustomTileData data, TileBase tile)
    {
        ID = data.ID;
        Type = (TileType)data.Type;
        Tile = tile;
        Icon = GetPreviewSprite(tile);
        UnlockLevel = data.UnlockLevel;
        Cost = data.Cost;
    }
    
    private Sprite GetPreviewSprite(TileBase tileBase)
    {
        if (tileBase is AnimatedTile animated)
        {
            return animated.m_AnimatedSprites.Length > 0 
                ? animated.m_AnimatedSprites[0] 
                : null;
        }

        return (tileBase as Tile)?.sprite;
    }
}
