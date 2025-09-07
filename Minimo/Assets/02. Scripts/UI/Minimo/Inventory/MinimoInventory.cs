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
        if (Slots == null) return;
        
        var sorted = Slots.OrderBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByLevel(bool desc)    
    {
        if (Slots == null) return;
        
        var sorted = desc
            ? Slots.OrderBy(slot => slot.Item.Level).ThenBy(slot => slot.Item.ID).ToList()
            : Slots.OrderByDescending(slot => slot.Item.Level).ThenBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByAcquisitionDate(bool desc)
    {
        if (Slots == null) return;
        
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
    public void FilterMinimo(bool[] activeArray)
    {
        if (Slots == null) return;
        
        foreach (var slot in Slots)
        {
            var buildingMatches = 
                (activeArray[0] && slot.Item.AssignedBuilding != null)
                || (activeArray[1] && slot.Item.AssignedBuilding == null);
            
            var typeMatches = 
                (activeArray[2] && slot.Item.Type == 0)
                || (activeArray[3] && slot.Item.Type == 1);
            
            slot.gameObject.SetActive(buildingMatches && typeMatches);
        }
    }
    #endregion
}
