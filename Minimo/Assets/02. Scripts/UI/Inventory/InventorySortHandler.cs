using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

public abstract class InventorySortHandler : MonoBehaviour
{
    protected class SortOption
    {
        public readonly string Label;
        public readonly Action Action;
        public SortOption(string label, Action action)
        {
            Label  = label;
            Action = action;
        }
    }
    
    [SerializeField] protected TMP_Dropdown Dropdown;

    protected List<SortOption> OptionList;
    
    private void Awake()
    {
        InitTabOptions();
        CacheOptionData();

        Dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    protected abstract void InitTabOptions();

    private void CacheOptionData()
    {
        var optionDatas = OptionList
            .Select(map => new TMP_Dropdown.OptionData(map.Label))
            .ToList();
        Dropdown.options = optionDatas;
    }

    private void OnDropdownChanged(int index)
    {
        OptionList[index].Action?.Invoke();
    }
}
