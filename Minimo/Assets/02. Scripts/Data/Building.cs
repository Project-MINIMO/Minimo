using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    public bool IsLocked => AccountInfo.Instance.level < UnlockLevel;
    
    public BuildingPositionData Position { get; private set; }
    public Sprite Icon { get; private set; }
    
    public Building(BuildingData data, TitleData title)
    {
        ID = data.ID;
        Code = data.Name;
        Type = (BuildingType)data.Type;
        UnlockLevel = data.UnlockLevel;
        Name = title.GetString($"STR_BUILDING_{data.Name.ToUpper()}_NAME");
        Description = title.GetString($"STR_BUILDING_{data.Name.ToUpper()}_DESC");
        Cost = data.Cost;
        Duration = data.Duration;

        LoadPositionData(data.Name);
    }

    private async Task LoadPositionData(string assetName)
    {
        var path = $"Assets/09. Scriptable Objects/Building/{assetName}.asset";
        var handle = Addressables.LoadAssetAsync<BuildingPositionData>(path);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Position = handle.Result;
            Icon = Position.Sprite;
        }
        else
        {
            Debug.LogError("Failed to load BuildingData");
        }
    }
}
