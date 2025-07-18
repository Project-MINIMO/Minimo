using UnityEngine;

public class ItemSlot : InventorySlot<Item>
{
    public override bool CanShow() => Item is { Count: > 0 };
    
    [SerializeField] private ItemInfoUpdater _info;
    
    private void OnEnable()
    {
        SetCount();
    }

    public override void Initialize(Item item)
    {
        base.Initialize(item);

        Item = item;
        Item.OnItemCountChnaged += SetCount;
        _info.UpdateItem(item);
    }

    private void SetCount()
    {
        if (Item == null) return;
        
        gameObject.SetActive(CanShow());
        _info.UpdateItemCount(Item.Count);
    }
}
