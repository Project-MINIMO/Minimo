using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySortHandler : MonoBehaviour
{
    private class SortOption
    {
        public readonly string Label;
        public readonly Action Action;
        public SortOption(string label, Action action)
        {
            Label  = label;
            Action = action;
        }
    }
    
    [SerializeField] private StorageInventory _inventory;
    [SerializeField] private TMP_Dropdown _dropdown;

    private Dictionary<int, List<SortOption>> _optionMap;
    private Dictionary<int, List<TMP_Dropdown.OptionData>> _cachedOptionData;

    private int _selectedIndex = -1;
    
    private void Awake()
    {
        InitTabOptions();
        CacheOptionData();

        _dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void InitTabOptions()
    {
        var titleData = App.GetData<TitleData>();
        
        _optionMap = new Dictionary<int, List<SortOption>>
        {
            [0] = new()
            {
                new SortOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT1_NAME"), _inventory.SortDefault),
                new SortOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT2_NAME") + " ↓", ()=> _inventory.SortByCount(false)),
                new SortOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT2_NAME")+ " ↑", ()=> _inventory.SortByCount(true)),
                new SortOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT3_NAME") + " ↓", ()=> _inventory.SortByPrice(false)),
                new SortOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT3_NAME")+ " ↑", ()=> _inventory.SortByPrice(true)),
            },
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
        _dropdown.value = 0;
        _dropdown.RefreshShownValue();
        
        _optionMap[index][0].Action?.Invoke();
    }

    private void OnDropdownChanged(int index)
    {
        var option = _optionMap[_selectedIndex][index];
        option.Action?.Invoke();
    }
}
