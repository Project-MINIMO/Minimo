using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
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
    
    [SerializeField] protected FilterDropdown Dropdown;
    [SerializeField] private Image _labelImg;
    [SerializeField] private Sprite _clearSprite;
    [SerializeField] private TextMeshProUGUI _labelTMP;
    
    protected Dictionary<int, List<FilterOption>> OptionMap;
    protected Dictionary<int, Action<bool[]>> ActionMap;
    protected Dictionary<int, List<TMP_Dropdown.OptionData>> CachedOptionData;

    protected int SelectedIndex = 1;

    protected virtual void Awake()
    {
        InitTabOptions();
        CacheOptionData();

        Dropdown.OnSelectionChanged.AddListener(OnDropdownChanged);
        Dropdown.SetLabel(_labelImg, _clearSprite);
    }

    protected virtual void InitTabOptions()
    {
        var titleData = App.GetData<TitleData>();
        _labelTMP.SetText(titleData.GetString("STR_STORTAGE_UI_FILTER_NAME"));
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