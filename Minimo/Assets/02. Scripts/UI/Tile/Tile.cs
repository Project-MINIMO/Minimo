using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile 
{
    public readonly int ID;
    public readonly TileType Type;
    public readonly Tile Tile;
    public readonly Sprite Icon;
    public readonly int UnlockLevel;
    public readonly int Cost;

    public bool IsLocked => AccountInfo.Instance.Level.Count < UnlockLevel;
    
    public CustomTile(CustomTileData data, Tile tile)
    {
        ID = data.ID;
        Type = (TileType)data.Type;
        Tile = tile;
        Icon = tile.sprite;
        UnlockLevel = data.UnlockLevel;
        Cost = data.Cost;
    }

    public bool CanInstall()
    {
        return AccountInfo.Instance.Gold.Count >= Cost;
    }
}
