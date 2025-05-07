using UnityEngine;

public enum ItemType
{
    None,
    Seed,
    Harvest,
    Ingredient,
    Product,
    Construction
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string Code;
    public int ID { get; private set; }

    public ItemData Data { get; private set; }
    public Sprite Icon { get; private set; }

    public void SetData(ItemData _data)
    {
        Data = _data;
        ID = _data.ID;

        Icon = Resources.Load<Sprite>($"Item/{_data.ID}");
    }
}