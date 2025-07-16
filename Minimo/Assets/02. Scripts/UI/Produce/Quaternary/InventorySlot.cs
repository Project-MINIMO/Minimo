using System;

using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public bool CanShow => Item is { Count: > 0 };
    public Item Item { get; private set; }
    
    public event Action<Item> OnItemSelected;

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
        _info.UpdateItem(item, item.Count);
        _inventoryBtn.onClick.AddListener(() => OnItemSelected?.Invoke(Item));
    }

    private void SetCount()
    {
        if (Item == null) return;
        
        gameObject.SetActive(CanShow);
        _info.UpdateItemCount(Item.Count);
    }
}
