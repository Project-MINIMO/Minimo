using System.Collections.Generic;

using UnityEngine;

public class RandomItemSelector
{
    private readonly TitleData _titleData = App.GetData<TitleData>();

    public List<ItemData> GetRandomItems(ResourceType type, int count = 1)
    {
        var items = new List<ItemData>();

        for (var i = 0; i < count; i++) 
        {
            var item = GetRandomItem(type);
            items.Add(item);
        }

        return items;
    }
    
    private ItemData GetRandomItem(ResourceType type)
    {
        var randomIndex = Random.Range(0, _titleData.Item.Count);
        return _titleData.Item[randomIndex];
    }
}
