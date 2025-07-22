using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using TMPro;

public abstract class InventoryFilterHandler : MonoBehaviour
{
    protected class FilterOption
    {
        public readonly string Label;

        public FilterOption(string label)
        {
            Label = label;
        }
    }
    
    [SerializeField] protected MultiSelectDropdown Dropdown;
    [SerializeField] private TextMeshProUGUI _label;
    
    protected Dictionary<int, List<FilterOption>> OptionMap;
    protected Dictionary<int, Action<bool[]>> ActionMap;
    protected Dictionary<int, List<TMP_Dropdown.OptionData>> CachedOptionData;

    protected int SelectedIndex = 1;

    private void Awake()
    {
        InitTabOptions();
        CacheOptionData();

        Dropdown.OnSelectionChanged.AddListener(OnDropdownChanged);
    }

    protected virtual void InitTabOptions()
    {
        var titleData = App.GetData<TitleData>();
        _label.SetText(titleData.GetString("STR_STORTAGE_UI_FILTER_NAME"));
    }

    private void CacheOptionData()
    {
        CachedOptionData = new Dictionary<int, List<TMP_Dropdown.OptionData>>();
        foreach (var map in OptionMap)
        {
            CachedOptionData[map.Key] =
                map.Value.Select(option => new TMP_Dropdown.OptionData(option.Label)).ToList();
        }
        
        Dropdown.options = CachedOptionData[1];
    }

    private void OnDropdownChanged(bool[] index)
    {
        ActionMap[SelectedIndex]?.Invoke(index);
    }
}