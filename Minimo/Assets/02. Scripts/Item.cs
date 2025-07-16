using UnityEngine;

public class Item
{
    public readonly ItemData Data;
    
    public readonly string Name;
    public readonly string Description;
    public readonly Sprite Icon;
    
    public int Count { get; private set; }
    
    public Item(ItemData data)
    {
        Data = data;

        Name = App.GetData<TitleData>().GetString($"STR_ITEM_{data.Name.ToUpper()}_NAME");
        Description = App.GetData<TitleData>().GetString($"STR_ITEM_{data.Name.ToUpper()}_DESC");
        Icon = Resources.Load<Sprite>($"Item/{data.Name}");
    }

    public void AddCount(int num)
    {
        Count += num;
        
        Count = Mathf.Clamp(Count, 0, 99999);
    }
}
