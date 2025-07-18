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

    private List<SortOption> _optionList;
    
    private void Awake()
    {
        InitTabOptions();
        CacheOptionData();

        _dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void InitTabOptions()
    {
        var titleData = App.GetData<TitleData>();
        
        _optionList = new List<SortOption>
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

    private void CacheOptionData()
    {
        var optionDatas = _optionList
            .Select(map => new TMP_Dropdown.OptionData(map.Label))
            .ToList();
        _dropdown.options = optionDatas;
    }

    public void OnMenuChanged(int index)
    {
        if (index != 0)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        
        _dropdown.value = 0;
        _dropdown.RefreshShownValue();
    }

    private void OnDropdownChanged(int index)
    {
        _optionList[index].Action?.Invoke();
    }
}
