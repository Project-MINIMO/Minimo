using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryDropdown : MonoBehaviour
{
    private class DropdownOption
    {
        public readonly string Label;
        public readonly Action Action;
        public DropdownOption(string label, Action action)
        {
            Label  = label;
            Action = action;
        }
    }
    
    [SerializeField] private StorageInventory _inventory;
    [SerializeField] private TMP_Dropdown _dropdown;

    private Dictionary<int, List<DropdownOption>> _optionMap;
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
        
        _optionMap = new Dictionary<int, List<DropdownOption>>
        {
            [0] = new()
            {
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT1_NAME"), _inventory.SortDefault),
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT2_NAME") + " ↓", ()=> _inventory.SortByCount(false)),
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT2_NAME")+ " ↑", ()=> _inventory.SortByCount(true)),
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT3_NAME") + " ↓", ()=> _inventory.SortByPrice(false)),
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_ALIGN_COMPONENT3_NAME")+ " ↑", ()=> _inventory.SortByPrice(true)),
            },
            [1] = new()
            {
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT1_NAME"), ()=> _inventory.FilterItem(0)),
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT2_NAME"), ()=> _inventory.FilterItem(1)),
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT3_NAME"), ()=> _inventory.FilterItem(2)),
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT4_NAME"), ()=> _inventory.FilterItem(3)),
            },
            [2] = new()
            {
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT1_NAME"), ()=> _inventory.FilterItem(0)),
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT5_NAME"), ()=> _inventory.FilterItem(1)),
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT6_NAME"),   ()=> _inventory.FilterItem(2)),
            },
            [3] = new()
            {
                new DropdownOption(titleData.GetString("STR_STORTAGE_UI_FILTER_COMPONENT1_NAME"), ()=> _inventory.FilterProps(ItemProperty.None)),
                new DropdownOption("사랑", ()=> _inventory.FilterProps(ItemProperty.Love)),
                new DropdownOption("우정", ()=> _inventory.FilterProps(ItemProperty.Friendship)),
                new DropdownOption("추억", ()=> _inventory.FilterProps(ItemProperty.Memory)),
                new DropdownOption("안정", ()=> _inventory.FilterProps(ItemProperty.Peace)),
                new DropdownOption("희망", ()=> _inventory.FilterProps(ItemProperty.Hope)),
                new DropdownOption("용기", ()=> _inventory.FilterProps(ItemProperty.Courage)),
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
        if (index > _cachedOptionData.Count - 1)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        
        _selectedIndex = index;
        
        _dropdown.options = _cachedOptionData[index];
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
