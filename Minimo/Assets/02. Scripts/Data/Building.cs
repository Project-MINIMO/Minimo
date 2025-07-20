using UnityEngine;

public class Building
{
    public readonly int ID;
    public readonly string Code;
    public readonly BuildingType Type;
    public readonly int UnlockLevel;
    public readonly string Name;
    public readonly string Description;
    public readonly int Cost;
    public readonly int Duration;
    public readonly BuildingPositionData Position;
    public readonly Sprite Icon;
    
    public bool IsLocked => AccountInfo.Instance.level < UnlockLevel;
    
    public Building(BuildingData data, BuildingPositionData position, TitleData title)
    {
        ID = data.ID;
        Code = data.Name;
        Type = (BuildingType)data.Type;
        UnlockLevel = data.UnlockLevel;
        Name = title.GetString($"STR_BUILDING_{data.Name.ToUpper()}_NAME");
        Description = title.GetString($"STR_BUILDING_{data.Name.ToUpper()}_DESC");
        Cost = data.Cost;
        Duration = data.Duration;
        Position = position;
        Icon = position.Sprite;
    }
}
