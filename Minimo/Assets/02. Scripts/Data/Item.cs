using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    public Sprite Icon { get; private set; }
    
    public int Count { get; private set; }
    
    public Item(ItemData data, TitleData title)
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
        LoadIcon(data.Name);
    }

    private async Task LoadIcon(string assetName)
    {
        var path = $"Assets/03. Images/Item/{assetName}.png";
        var handle = Addressables.LoadAssetAsync<Sprite>(path);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Icon = handle.Result;
        }
        else
        {
            Debug.LogError($"Failed to load Item Icon : {assetName}");
        }
    }

    public void AddCount(int num)
    {
        Count += num;
        
        Count = Mathf.Clamp(Count, 0, 99999);
        OnItemCountChanged?.Invoke();
    }
}
