using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderSlot : MonoBehaviour
{
    public event Action<OrderSlot> OnSlotSelected;
    public Item Item { get; private set; }
    public int Amount => OrderManager.Instance.OrderItems[Item.ID];
    
    [SerializeField] private Button _slotBtn;
    [SerializeField] private ItemInfoUpdater _itemInfo;
    [SerializeField] private ItemInfoUpdater[] _materialInfos;
    
    private void Start()
    {
        _slotBtn.onClick.AddListener(() => OnSlotSelected?.Invoke(this));
    }
    
    public void SetItem(Item item)
    {
        Item = item;
        
        _itemInfo.UpdateItem(item);
        var i = 0;
        for (; i < item.MaterialCodes.Length; i++)
        {
            _materialInfos[i].gameObject.SetActive(true);
            _materialInfos[i].UpdateItem(item.MaterialCodes[i]);
        }
        for (; i < _materialInfos.Length; i++)
        {
            _materialInfos[i].gameObject.SetActive(false);
        }
    }
}
