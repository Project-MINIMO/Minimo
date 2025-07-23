using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class FilterDropdown : TMP_Dropdown
{
    private Image _labelImg;
    private Sprite _clearSprite;
    private Sprite _startSprite;
    private GameObject[] _backgroundObjs;
    
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
    }
    
    protected override GameObject CreateDropdownList(GameObject template)
    {
        _labelImg.sprite = _clearSprite;

        if (_backgroundObjs != null)
        {
            var selectedIndex = GetBackgroundIndex(_selectedStates.Length);
            for (var i = 0; i < _backgroundObjs.Length; i++)
            {
                _backgroundObjs[i].SetActive(i == selectedIndex);
            }
        }
        
        return base.CreateDropdownList(template);
    }
    
    private int GetBackgroundIndex(int count) => count switch
    {
        3 => 0,
        2 => 1,
        _ => 2,
    };

    protected override void DestroyDropdownList(GameObject dropdownList)
    {
        base.DestroyDropdownList(dropdownList);

        _labelImg.sprite = _startSprite;
        
        if (_backgroundObjs != null)
        {
            foreach (var obj in _backgroundObjs)
            {
                obj.SetActive(false);
            }
        }
    }

    public void SetLabel(Image label, Sprite sprite)
    {
        _labelImg = label;
        _clearSprite = sprite;
        _startSprite = _labelImg.sprite;
    }

    public void SetBackgroundObjects(GameObject[] backgroundObjs)
    {
        _backgroundObjs = backgroundObjs;
    }
}