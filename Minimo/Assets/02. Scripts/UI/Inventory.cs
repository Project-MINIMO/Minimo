using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public abstract class Inventory<T> : MonoBehaviour
{
    [SerializeField] protected Toggle[] _menuTogs;
    [SerializeField] private ScrollRect _scrollRect;

    private List<InventorySlot<T>> _slots;
    
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
        var existingButtons = _scrollRect.GetComponentsInChildren<InventorySlot<T>>(true);

        var filteredItems = GetFilteredItems();
        _slots = new List<InventorySlot<T>>(filteredItems.Count);
        
        var i = 0;
        
        for (; i < filteredItems.Count; i++)
        {
            var slot = existingButtons[i];
            slot.Initialize(filteredItems[i]);
            slot.gameObject.SetActive(true);
            
            _slots.Add(slot);
        }

        for (; i < existingButtons.Length; i++)
        {
            existingButtons[i].gameObject.SetActive(false);
        }
    }
    
    protected abstract void SetString();
    
    protected abstract List<T> GetFilteredItems();

    private void FilterSlots(int index)
    {
        foreach (var button in _slots)
        {
            var isActive = IsSlotFiltered(index, button.Item);
            
            button.gameObject.SetActive(isActive);
        }
        
        _scrollRect.verticalNormalizedPosition = 1;
    }
    
    protected abstract bool IsSlotFiltered(int index, T item);
}
