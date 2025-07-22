using System.Linq;
using System.Collections.Generic;

using UnityEngine.UI;

public class MinimoInventory : Inventory<Minimo>
{
    protected override void SetString(){ }
    protected override bool IsSlotFiltered(int index, Minimo item) => false;
    
    protected override List<Minimo> GetFilteredItems() => App.GetManager<MinimoManager>().Minimos;

    #region Sort
    public void SortDefault()
    {
        var sorted = Slots.OrderBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByCount(bool desc)    
    {
        var sorted = desc
            ? _activeSlots.OrderBy(slot => slot.Item.Count).ThenBy(slot => slot.Item.ID).ToList()
            : _activeSlots.OrderByDescending(slot => slot.Item.Count).ThenBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByPrice(bool desc)
    {
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
        foreach (var slot in _activeSlots)
        {
            var isActive = activeArray[slot.Item.Level - 1];
            
            slot.gameObject.SetActive(isActive);
        }
    }
    public void FilterProps(bool[] activeArray)
    {
        foreach (var slot in _activeSlots)
        {
            var isActive = activeArray[(int)slot.Item.Property - 1];
            
            slot.gameObject.SetActive(isActive);
        }
    }
    #endregion
}
