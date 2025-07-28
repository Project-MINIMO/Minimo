using System;

using UnityEngine;

public class Item : IQuestClearTarget, IQuestRewardTarget
{
    public event Action OnItemCountChanged;
    
    public string Name { get; }
    public int Count { get; private set; }
    public Sprite Icon { get; }
    
    public readonly int ID;
    public readonly string Code;
    public readonly ItemType Type;
    public readonly int Level;
    public readonly ItemProperty Property;
    public readonly int SellCost;
    public readonly int BuyCost;
    public readonly string Description;
    
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

    public void AddCount(int amount)
    {
        Count += amount;
        Count = Mathf.Clamp(Count, 0, int.MaxValue);
        OnItemCountChanged?.Invoke();
    }
}
