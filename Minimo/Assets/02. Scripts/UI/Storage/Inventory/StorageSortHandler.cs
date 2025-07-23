using System.Collections.Generic;

using UnityEngine;

public class StorageSortHandler : InventorySortHandler
{
    [SerializeField] protected StorageInventory _inventory;
    
    protected override void InitTabOptions()
    {
        var titleData = App.GetData<TitleData>();
        
        OptionList = new List<SortOption>
        {
            new(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT1_NAME"), 
                _inventory.SortDefault),
            new(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT2_NAME") + " ↓", 
                () => _inventory.SortByCount(false)),
            new(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT2_NAME")+ " ↑", 
                () => _inventory.SortByCount(true)),
            new(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT3_NAME") + " ↓", 
                () => _inventory.SortByPrice(false)),
            new(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT3_NAME")+ " ↑", 
                () => _inventory.SortByPrice(true))
        };
    }

    public void OnMenuChanged(int index)
    {
        if (index != 0)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        
        Dropdown.value = 0;
        Dropdown.RefreshShownValue();
    }
}
