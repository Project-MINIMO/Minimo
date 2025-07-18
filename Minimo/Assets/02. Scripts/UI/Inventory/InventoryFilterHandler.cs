using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryFilterHandler : MonoBehaviour
{
    private class FilterOption
    {
        public readonly string Label;

        public FilterOption(string label)
        {
            Label = label;
        }
    }

    [SerializeField] private StorageInventory _inventory;
    [SerializeField] private MultiSelectDropdown _dropdown;

    private Dictionary<int, List<FilterOption>> _optionMap;
    private Dictionary<int, Action<bool[]>> _actionMap;
    private Dictionary<int, List<TMP_Dropdown.OptionData>> _cachedOptionData;

    private int _selectedIndex = -1;

    private void Awake()
    {
        InitTabOptions();
        CacheOptionData();

        _dropdown.onMultiValueChanged.AddListener(OnDropdownChanged);
    }

    private void InitTabOptions()
    {
        var titleData = App.GetData<TitleData>();

        _optionMap = new Dictionary<int, List<FilterOption>>
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

        _actionMap = new Dictionary<int, Action<bool[]>>
        {
            [1] = _inventory.FilterItem,
            [2] = _inventory.FilterItem,
            [3] = _inventory.FilterProps,
        };
    }

    private void CacheOptionData()
    {
        _cachedOptionData = new Dictionary<int, List<TMP_Dropdown.OptionData>>();
        foreach (var map in _optionMap)
        {
            _cachedOptionData[map.Key] =
                map.Value.Select(option => new TMP_Dropdown.OptionData(option.Label)).ToList();
        }
    }

    public void OnMenuChanged(int index)
    {
        if (_cachedOptionData == null) return;
        if (!_cachedOptionData.TryGetValue(index, out var list))
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        _selectedIndex = index;

        _dropdown.options = list;
        var count = list.Count;
        var refreshBools = Enumerable
            .Repeat(true, count)
            .ToArray();
        _dropdown.multiValue = refreshBools;

        _actionMap[index]?.Invoke(refreshBools);
    }

    private void OnDropdownChanged(bool[] index)
    {
        var option = _actionMap[_selectedIndex];
        option?.Invoke(index);
    }
}