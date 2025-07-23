using System.Collections.Generic;
using UnityEngine;

public class MinimoSortHandler : InventorySortHandler
{
    [SerializeField] protected MinimoInventory _inventory;
    
    protected override void InitTabOptions()
    {
        var titleData = App.GetData<TitleData>();
        
        OptionList = new List<SortOption>
        {
            new(titleData.GetString("STR_MC_ALIGN_COMPONENT1_NAME"), 
                _inventory.SortDefault),
            new(titleData.GetString("STR_MC_ALIGN_COMPONENT2_NAME") + " ↓", 
                () => _inventory.SortByLevel(false)),
            new(titleData.GetString("STR_MC_ALIGN_COMPONENT2_NAME")+ " ↑", 
                () => _inventory.SortByLevel(true)),
            new(titleData.GetString("STR_MC_ALIGN_COMPONENT3_NAME") + " ↓", 
                () => _inventory.SortByAcquisitionDate(false)),
            new(titleData.GetString("STR_MC_ALIGN_COMPONENT3_NAME")+ " ↑", 
                () => _inventory.SortByAcquisitionDate(true))
        };
    }
}