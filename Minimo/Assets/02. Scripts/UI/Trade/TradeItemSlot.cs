using UnityEngine;

public class TradeItemSlot : InventorySlot<Item>
{
    public override bool CanShow() => _building.Count > 0 && !TradeManager.Instance.TradeItems.Contains(Item);
    
    [SerializeField] private ItemInfoUpdater _info;
    [SerializeField] private GameObject[] _slotImgs;
    
    private Building _building;
    
    private void OnEnable()
    {
        SetCount();
    }
        
    public override void Initialize(Item item)
    {
        base.Initialize(item);

        Item = item;
        Item.OnItemCountChanged += SetCount;

        var titleData = App.GetData<TitleData>();
        _building = titleData.Building[item.BuildingCode];
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
