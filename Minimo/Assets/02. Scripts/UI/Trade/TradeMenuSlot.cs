using System;
using UnityEngine;
using UnityEngine.UI;

public class TradeMenuSlot : MonoBehaviour
{
    public event Action<TradeMenuSlot> OnSlotSelected;
    public Item Item { get; private set; }
    
    [SerializeField] private Button _slotBtn;
    [SerializeField] private ItemInfoUpdater _itemInfo;
    
    private void Start()
    {
        _slotBtn.onClick.AddListener(() => OnSlotSelected?.Invoke(this));
    }

    public void SetItem(Item item)
    {
        Item = item;
        
        _itemInfo.gameObject.SetActive(true);
        _itemInfo.UpdateItem(item);
    }
    
    public void ClearItem()
    {
        Item = null;
        
        _itemInfo.gameObject.SetActive(false);
        _itemInfo.ClearItem();
    }
}
