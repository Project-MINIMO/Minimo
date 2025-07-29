using System;
using System.Linq;

using System.Collections.Generic;
using UnityEngine;

public class AccountInfo : Singleton<AccountInfo>
{
    public Dictionary<int, Item> Items { get; private set; }
    public UserLevel Level { get; private set; }
    public UserGold Gold { get; private set; }
    public int Cash;
    public int StorageCapacity { get; private set; } = 100;
    public int MinimoCapacity { get; private set; } = 100;
    public int Star;
    public bool IsAutoAssign;

    private GetItemPanel _itemPanel;
    public event Action<int> OnStorageCapacityChanged;
    public event Action<int> OnMinimoCapacityChanged;
    public int CurrentItemCounts => Items.Values.Where(item => item.Level > 0).Sum(item => item.Count);
    public event Action<bool> OnAutoAssign;
    
    [SerializeField] private Sprite LevelIcon;
    [SerializeField] private Sprite GoldIcon;

    private void Start()
    {
        Level = new UserLevel(LevelIcon);
        Gold = new UserGold(GoldIcon);
    }

    public void AddItems(Dictionary<int, Item> items)
    {
        Items = items;
    }
    
    public bool CanKeepItem(int id)
    {
        var cankeep = CurrentItemCounts < StorageCapacity;
        if (!cankeep)
        {
            App.Notification(NotifyType.CapacityLack);
        }

        return cankeep;
    }

    public void AddItem(int id, int amount)
    {
        Items[id].AddCount(amount);

        if (_itemPanel == null)
        {
            _itemPanel = App.GetManager<UIManager>().GetItem;
        }
        _itemPanel.EnqueueItem(id);
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }

    public void AddItem(Item item, int amount)
    {
        item.AddCount(amount);
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }

    public void RemoveItem(int id, int amount)
    {
        Items[id].AddCount(-amount);
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }
    
    public void RemoveItem(Item item, int amount)
    {
        item.AddCount(-amount);
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }

    public void AddStorageCapacity(int amount)
    {
        StorageCapacity += amount;
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }
    
    public void AddMinimoCapacity(int amount)
    {
        MinimoCapacity += amount;
        OnMinimoCapacityChanged?.Invoke(MinimoCapacity);
    }

    public void AutoAssign(bool isAutoAssign)
    {
        IsAutoAssign = isAutoAssign;
        OnAutoAssign?.Invoke(isAutoAssign);
    }

    [ContextMenu("AddGold1000000")]
    public void AddGold1000000()
    {
        Gold.AddCount(1000000);
    }
    
    [ContextMenu("AddLevel1")]
    public void AddLevel1()
    {
        Level.AddCount(100);
    }
}
