using UnityEngine;

public class ItemSlot : InventorySlot<Item>
{
    public override bool CanShow() => Item is { Count: > 0 };
    
    [SerializeField] private ItemInfoUpdater _info;
    [SerializeField] private GameObject[] _slotImgs;
    
    private void OnEnable()
    {
        SetCount();
    }

    public override void Initialize(Item item)
    {
        base.Initialize(item);

        Item = item;
        Item.OnItemCountChanged += SetCount;
        _info.UpdateItem(item);

        if (item.Level < 4) return;
        
        _slotImgs[0].SetActive(false);
        _slotImgs[(int)item.Property].gameObject.SetActive(true);
    }

    private void SetCount()
    {
        if (Item == null) return;
        
        gameObject.SetActive(CanShow());
        _info.UpdateItemCount(Item.Count);
    }
}
