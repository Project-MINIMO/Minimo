using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountInfo : Singleton<AccountInfo>
{
    private TitleData _titleData;
    
    public Dictionary<int, Item> Items { get; } = new();
    public int level => Exp / 100;
    public int Gold;
    public int Cash;
    public int Exp = 100;
    public int Capacity { get; private set; } = 100;

    private GetItemPanel _itemPanel;
    public event Action<int> OnCapacityChanged;
    public int CurrentItemCounts => Items.Values.Count(item => item.Count > 0);

    private void Start()
    {
        _titleData = App.GetData<TitleData>();

        for (var i = 0; i < _titleData.Item.Count; i++)
        {
            var item = _titleData.Item[i];
            Items.TryAdd(item.ID, item);
        }
    }
    
    public void AddExp(int amount)
    {
        Exp += amount;
    }

    public bool CanKeepItem(int id)
    {
        if (Items[id].Count > 0) return true;
        
        var cankeep = CurrentItemCounts < Capacity;
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
        OnCapacityChanged?.Invoke(Capacity);
    }

    public void AddItem(Item item, int amount)
    {
        item.AddCount(amount);
        OnCapacityChanged?.Invoke(Capacity);
    }

    public void RemoveItem(int id, int amount)
    {
        Items[id].AddCount(-amount);
        OnCapacityChanged?.Invoke(Capacity);
    }
    
    public void RemoveItem(Item item, int amount)
    {
        item.AddCount(-amount);
        OnCapacityChanged?.Invoke(Capacity);
    }

    public void AddCapacity(int amount)
    {
        Capacity += amount;
        OnCapacityChanged?.Invoke(Capacity);
    }
}
