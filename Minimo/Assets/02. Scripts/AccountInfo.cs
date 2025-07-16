using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountInfo : Singleton<AccountInfo>
{
    private TitleData _titleData;
    
    public Dictionary<int, Item> Items { get; } = new();
    public int level { get; private set; } = 2;
    public int blueStar;
    public int rainbowStar;
    public int Exp { get; private set; }

    private GetItemPanel _itemPanel;

    private void Start()
    {
        _titleData = App.GetData<TitleData>();

        for (var i = 0; i < _titleData.Item.Count; i++)
        {
            var item = _titleData.Item[i];
            Items.TryAdd(item.ID, new Item(item));
        }
    }
    
    public void AddExp(int amount)
    {
        Exp += amount;
        level = Exp / 100;
    }

    public void AddItem(int id, int amount)
    {
        Items[id].AddCount(amount);

        if (_itemPanel == null)
        {
            _itemPanel = App.GetManager<UIManager>().GetItem;
        }
        _itemPanel.EnqueueItem(id);
    }

    public void AddItem(Item item, int amount)
    {
        item.AddCount(amount);
    }

    public void RemoveItem(int id, int amount)
    {
        Items[id].AddCount(-amount);
    }
    
    public void RemoveItem(Item item, int amount)
    {
        item.AddCount(-amount);
    }
}
