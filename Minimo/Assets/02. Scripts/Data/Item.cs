using System;

using UnityEngine;

public class Item
{
    public event Action OnItemCountChanged;
    
    public readonly int ID;
    public readonly string Code;
    public readonly ItemType Type;
    public readonly int Level;
    public readonly ItemProperty Property;
    public readonly int SellCost;
    public readonly int BuyCost;
    public readonly string Name;
    public readonly string Description;
    public readonly Sprite Icon;
    
    public int Count { get; private set; }
    
    public Item(ItemData data, Sprite icon, TitleData title)
    {
        ID = data.ID;
        Code = data.Name;
        Type = (ItemType)data.Type;
        Level = data.Level;
        Property = (ItemProperty)data.Property;
        SellCost = data.SellCost;
        BuyCost = data.BuyCost;
        Name = title.GetString($"STR_ITEM_{data.Name.ToUpper()}_NAME");
        Description = title.GetString($"STR_ITEM_{data.Name.ToUpper()}_DESC");
        Icon = icon;
    }

    public void AddCount(int num)
    {
        Count += num;
        
        Count = Mathf.Clamp(Count, 0, 99999);
        OnItemCountChanged?.Invoke();
    }
}
