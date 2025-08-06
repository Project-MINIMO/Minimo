using UnityEngine;

public class Building : IQuestClearTarget
{
    public bool CanInstall => AccountInfo.Instance.Gold.Count >= Cost;
    
    public string Name { get; }
    public int Count { get; private set; }
    public Sprite Icon { get; }

    public readonly int ID;
    public readonly string Code;
    public readonly BuildingType Type;
    public readonly int UnlockLevel;
    public readonly string Description;
    public readonly int Cost;
    public readonly int Duration;
    public readonly BuildingPositionData Position;
    
    public bool IsLocked => AccountInfo.Instance.Level.Count < UnlockLevel;
    
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
    
    public void AddCount(int amount)
    {
        Count += amount;
        Count = Mathf.Clamp(Count, 0, int.MaxValue);
    }

    public void Install()
    {
        if (!CanInstall)
        {
            App.Notification(NotifyType.GoldLack);
            return;
        }
        
        AccountInfo.Instance.Gold.AddCount(-Cost);
    }

    public void Uninstall()
    {
        AccountInfo.Instance.Gold.AddCount(Cost);
    }
}
