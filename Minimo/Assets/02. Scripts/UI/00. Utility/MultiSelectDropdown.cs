using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiSelectDropdown : TMP_Dropdown
{
    public GameObject _backgroundObj;
    public GameObject _backgroundObj2;
    
    [Serializable]
    public class SelectionChangedEvent : UnityEvent<bool[]> { }
    
    private SelectionChangedEvent _onSelectionChanged = new();
    public SelectionChangedEvent OnSelectionChanged 
    { 
        get => _onSelectionChanged;
        set => _onSelectionChanged = value;
    }
    
    private readonly List<Toggle> _itemToggles = new();
    private bool[] _selectedStates;
    
    public bool[] SelectedStates
    {
        get => _selectedStates;
        set => SetSelectedStates(value);
    }

    private void SetSelectedStates(bool[] states, bool sendCallback = true)
    {
        if (Application.isPlaying && states == _selectedStates)
            return;

        _selectedStates = states;

        if (sendCallback)
        {
            UISystemProfilerApi.AddMarker("Dropdown.value", this);
            _onSelectionChanged.Invoke(_selectedStates);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        DisplayDropdown();
    }
    
    protected override DropdownItem CreateItem(DropdownItem itemTemplate)
    {
        var item = base.CreateItem(itemTemplate);
        _itemToggles.Add(item.GetComponentInChildren<Toggle>(true));
        return item;
    }

    private void DisplayDropdown()
    {
        _itemToggles.Clear();
        
        Show();
        
        for (var i = 0; i < _itemToggles.Count; i++)
        {
            var toggle = _itemToggles[i];

            var index = i; 
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(isOn =>
            {
                _selectedStates[index] = isOn;
                _onSelectionChanged.Invoke(_selectedStates);
            });

            toggle.isOn = _selectedStates[index];
            if (_selectedStates[index])
            {
                toggle.Select();
            }
        }

        if (_selectedStates.Length == 3) _backgroundObj.SetActive(true);
        if (_selectedStates.Length == 2) _backgroundObj2.SetActive(true);
    }

    protected override void DestroyDropdownList(GameObject dropdownList)
    {
        base.DestroyDropdownList(dropdownList);
        
        _backgroundObj.SetActive(false);
        _backgroundObj2.SetActive(false);
    }
}