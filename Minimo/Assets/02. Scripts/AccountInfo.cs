using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountInfo : Singleton<AccountInfo>
{
    private TitleData _titleData;
    
    public Dictionary<ItemData, int> Items { get; } = new();
    public int level { get; private set; } = 2;
    public int blueStar;
    public int rainbowStar;
    public int Exp { get; private set; }

    private void Start()
    {
        _titleData = App.GetData<TitleData>();
    }
    
    public void AddExp(int amount)
    {
        Exp += amount;
        level = Exp / 100;
    }

    public void AddItem(int id, int amount)
    {
        var item = _titleData.Item[id];
        
        if (!Items.TryAdd(item, amount))
        {
            Items[item] += amount;
        }
    }

    public void RemoveItem(int id, int amount)
    {
        var item = _titleData.Item[id];
        
        RemoveItem(item, amount);
    }

    public void RemoveItem(ItemData item, int amount)
    {
        if (!Items.ContainsKey(item)) return;
        
        Items[item] -= amount;

        if (Items[item] <= 0)
        {
            Items.Remove(item);
        }
    }
}
