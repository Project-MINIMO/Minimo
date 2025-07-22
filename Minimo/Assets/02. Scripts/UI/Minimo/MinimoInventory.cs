using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class MinimoInventory : Inventory<Minimo>
{
    [SerializeField] private RectTransform _content;
    
    protected override void SetString(){ }
    protected override bool IsSlotFiltered(int index, Minimo item) => false;
    
    protected override List<Minimo> GetFilteredItems() => App.GetData<TitleData>().UserMinimo.Values.ToList();

    #region Sort
    public void SortDefault()
    {
        var sorted = Slots.OrderBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByLevel(bool desc)    
    {
        var sorted = desc
            ? Slots.OrderBy(slot => slot.Item.Level).ThenBy(slot => slot.Item.ID).ToList()
            : Slots.OrderByDescending(slot => slot.Item.Level).ThenBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByAcquisitionDate(bool desc)
    {
        var sorted = desc
            ? Slots.OrderBy(slot => slot.Item.AcquisitionDate).ThenBy(slot => slot.Item.ID).ToList()
            : Slots.OrderByDescending(slot => slot.Item.AcquisitionDate).ThenBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    private void SortSlots(List<InventorySlot<Minimo>> slots)
    {
        for (var i = 0; i < slots.Count; i++)
        {
            slots[i].transform.SetSiblingIndex(i);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
    }
    #endregion

    #region Filter
    public void FilterByAssingedBuilding(bool isAssinged)
    {
        foreach (var slot in Slots)
        {
            var isActive = slot.Item.AssignedBuilding == isAssinged;
            
            slot.gameObject.SetActive(isActive);
        }
    }
    public void FilterByType(int type)
    {
        foreach (var slot in Slots)
        {
            var isActive = slot.Item.Type == type;
            
            slot.gameObject.SetActive(isActive);
        }
    }
    #endregion
}
