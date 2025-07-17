using System;

using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public bool CanShow => Item is { Count: > 0 };
    public Item Item { get; private set; }
    
    public event Action<InventorySlot> OnItemSelected;

    [SerializeField] private ItemInfoUpdater _info;
    [SerializeField] private Button _inventoryBtn;
    
    private void OnEnable()
    {
        SetCount();
    }

    public void Initialize(int id)
    {
        var item = AccountInfo.Instance.Items[id];
        Item = item;
        Item.OnItemCountChnaged += SetCount;
        _info.UpdateItem(item);
        _inventoryBtn.onClick.AddListener(() => OnItemSelected?.Invoke(this));
    }

    private void SetCount()
    {
        if (Item == null) return;
        
        gameObject.SetActive(CanShow);
        _info.UpdateItemCount(Item.Count);
    }
}
