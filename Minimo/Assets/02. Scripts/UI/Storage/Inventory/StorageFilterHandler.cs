using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class StorageInventoryFilterHandler : InventoryFilterHandler
{
    [SerializeField] protected StorageInventory _inventory;
    
    protected override void InitTabOptions()
    {
        base.InitTabOptions();
        
        var titleData = App.GetData<TitleData>();

        OptionMap = new Dictionary<int, List<FilterOption>>
        {
            [1] = new()
            {
                new FilterOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT2_NAME")),
                new FilterOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT3_NAME")),
                new FilterOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT4_NAME")),
            },
            [2] = new()
            {
                new FilterOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT5_NAME")),
                new FilterOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT6_NAME")),
            },
            [3] = new()
            {
                new FilterOption("사랑"),
                new FilterOption("우정"),
                new FilterOption("추억"),
                new FilterOption("안정"),
                new FilterOption("희망"),
                new FilterOption("용기"),
            },
        };

        ActionMap = new Dictionary<int, Action<bool[]>>
        {
            [1] = _inventory.FilterItem,
            [2] = _inventory.FilterItem,
            [3] = _inventory.FilterProps,
        };
    }

    public void OnMenuChanged(int index)
    {
        if (CachedOptionData == null) return;
        if (!CachedOptionData.TryGetValue(index, out var list))
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        SelectedIndex = index;

        Dropdown.options = list;
        var count = list.Count;
        var refreshBools = Enumerable
            .Repeat(true, count)
            .ToArray();
        Dropdown.SelectedStates = refreshBools;
    }
}
