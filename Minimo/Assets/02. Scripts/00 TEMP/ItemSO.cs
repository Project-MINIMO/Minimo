using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public List<Item> items;
    
    public Item GetItem(int id)
    {
        return items.Find(item => item.ID == id);
    }
    
    public Item GetItem(string code)
    {
        return items.Find(item => item.Code == code);
    }
}