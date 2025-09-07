using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class Inventory<T> : MonoBehaviour
{
    [SerializeField] private InventorySlot<T> _slotPrefab; 
    [SerializeField] private Transform _contentParent;   
    
    [SerializeField] protected Toggle[] _menuTogs;
    [SerializeField] private ScrollRect _scrollRect;

    protected List<InventorySlot<T>> Slots;
    
    private void Awake()
    {
        InitSlots();
        SetString();
        
        for (var i = 0; i < _menuTogs.Length; i++)
        {
            var index = i;
            _menuTogs[index].onValueChanged.AddListener((isOn) =>
            {
                if (isOn) FilterSlots(index);
            });
        }
    }
    
    private void InitSlots()
    {
        var existingButtons = _scrollRect
            .GetComponentsInChildren<InventorySlot<T>>(true)
            .ToList();

        var filteredItems = GetFilteredItems();

        Slots = new List<InventorySlot<T>>(filteredItems.Count);
        
        var existingCount = existingButtons.Count;
        var needCount = filteredItems.Count;
        
        for (var i = 0; i < needCount; i++)
        {
            InventorySlot<T> slot;

            if (i < existingCount)
            {
                slot = existingButtons[i];
            }
            else
            {
                slot = Instantiate(_slotPrefab, _contentParent);
                slot.transform.SetSiblingIndex(i);
            }

            slot.Initialize(filteredItems[i]);
            slot.gameObject.SetActive(true);
            Slots.Add(slot);
        }
        
        for (var i = needCount; i < existingCount; i++)
        {
            existingButtons[i].gameObject.SetActive(false);
        }
    }
    
    protected abstract void SetString();
    
    protected abstract List<T> GetFilteredItems();

    protected virtual void FilterSlots(int index)
    {
        foreach (var slot in Slots)
        {
            var isActive = IsSlotFiltered(index, slot.Item);
            
            slot.gameObject.SetActive(isActive);
        }
        
        _scrollRect.verticalNormalizedPosition = 1;
    }
    
    protected abstract bool IsSlotFiltered(int index, T item);
}
