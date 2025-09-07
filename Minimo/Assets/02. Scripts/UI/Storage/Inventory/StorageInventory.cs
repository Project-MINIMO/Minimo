using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class StorageInventory : ItemInventory
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private StorageSortHandler _sortDropdown;
    [SerializeField] private StorageFilterHandler _filterDropdown;
    
    private List<InventorySlot<Item>> _activeSlots;
   
    protected override void FilterSlots(int index)
    {
        base.FilterSlots(index);
        
        _activeSlots = Slots.Where(slot => slot.gameObject.activeSelf).ToList();
        SortDefault();
        
        _sortDropdown.OnMenuChanged(index);
        _filterDropdown.OnMenuChanged(index);
    }

    #region Sort
    public void SortDefault()
    {
        if (_activeSlots == null) return;
        
        var sorted = _activeSlots.OrderBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByCount(bool desc)    
    {
        if (_activeSlots == null) return;
        
        var sorted = desc
            ? _activeSlots.OrderBy(slot => slot.Item.Count).ThenBy(slot => slot.Item.ID).ToList()
            : _activeSlots.OrderByDescending(slot => slot.Item.Count).ThenBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByPrice(bool desc)
    {
        if (_activeSlots == null) return;
        
        var sorted = desc
            ? _activeSlots.OrderBy(slot => slot.Item.SellCost).ThenBy(slot => slot.Item.ID).ToList()
            : _activeSlots.OrderByDescending(slot => slot.Item.SellCost).ThenBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    private void SortSlots(List<InventorySlot<Item>> slots)
    {
        for (var i = 0; i < slots.Count; i++)
        {
            slots[i].transform.SetSiblingIndex(i);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
    }
    #endregion

    #region Filter
    public void FilterItem(bool[] activeArray)
    {
        if (_activeSlots == null) return;
        
        foreach (var slot in _activeSlots)
        {
            var isActive = activeArray[slot.Item.Level - 1];
            
            slot.gameObject.SetActive(isActive);
        }
    }
    public void FilterProps(bool[] activeArray)
    {
        if (_activeSlots == null) return;
        
        foreach (var slot in _activeSlots)
        {
            var isActive = activeArray[(int)slot.Item.Property - 1];
            
            slot.gameObject.SetActive(isActive);
        }
    }
    #endregion
}
