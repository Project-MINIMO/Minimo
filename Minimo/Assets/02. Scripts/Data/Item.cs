using System;

using UnityEngine;

public class Item : IQuestClearTarget, IQuestRewardTarget
{
    public event Action OnItemCountChanged;
    
    public string Name { get; }
    public int Count { get; private set; }
    public Sprite Icon { get; }
    public int BuildingCode { get; private set; }
    public int[] MaterialCodes { get; private set; }
    
    public readonly int ID;
    public readonly string Code;
    public readonly ItemType Type;
    public readonly int Level;
    public readonly ItemProperty Property;
    public readonly int SellCost;
    public readonly int BuyCost;
    public readonly int Exp;
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
        Exp = data.EXP;
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
    
    public void SetBuildingID(int buildingID) => BuildingCode = buildingID;
    public void SetMaterialCodes(int[] materialCodes)
    {
        if (Level == 1)
        {
            materialCodes = Array.Empty<int>();
        }
        
        MaterialCodes = materialCodes;
    }
}
