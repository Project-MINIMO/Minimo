using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class MinimoFilterHandler : InventoryFilterHandler
{
    [SerializeField] protected MinimoInventory _inventory;
    
    protected override void InitTabOptions()
    {
        base.InitTabOptions();
        
        var titleData = App.GetData<TitleData>();

        OptionMap = new Dictionary<int, List<FilterOption>>
        {
            [1] = new()
            {
                new FilterOption(titleData.GetString("STR_MINIMOCENTER_FILTER_COMPONENT2_NAME")),
                new FilterOption(titleData.GetString("STR_MINIMOCENTER_FILTER_COMPONENT3_NAME")),
                new FilterOption(titleData.GetString("STR_MINIMOCENTER_FILTER_COMPONENT4_NAME")),
                new FilterOption(titleData.GetString("STR_MINIMOCENTER_FILTER_COMPONENT5_NAME")),
            }
        };

        ActionMap = new Dictionary<int, Action<bool[]>>
        {
            [1] = _inventory.FilterMinimo,
        };
    }

    private void OnEnable()
    {
        var count = OptionMap[1].Count;
        var refreshBools = Enumerable
            .Repeat(true, count)
            .ToArray();
        Dropdown.SelectedStates = refreshBools;
    }
}
