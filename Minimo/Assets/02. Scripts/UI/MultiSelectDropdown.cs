using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiSelectDropdown : TMP_Dropdown
{
    [Serializable]
    public class BoolArrayEvent : UnityEvent<bool[]> { }
    
    private BoolArrayEvent _onValueArrayChanged = new();
    public BoolArrayEvent onMultiValueChanged 
    { 
        get => _onValueArrayChanged;
        set => _onValueArrayChanged = value;
    }
    
    private List<bool> _selectedIndices { get; } = new ();
    private List<Toggle> _toggles = new();

    private bool[] m_multiValue;
    
    public bool[] multiValue
    {
        get => m_multiValue;
        set => SetValue(value);
    }

    private void SetValue(bool[] value, bool sendCallback = true)
    {
        if (Application.isPlaying && value == m_multiValue)
            return;

        m_multiValue = value;
        RefreshShownValue();

        if (sendCallback)
        {
            UISystemProfilerApi.AddMarker("Dropdown.value", this);
            _onValueArrayChanged.Invoke(m_multiValue);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        ShowMulti();
    }
    
    protected override DropdownItem CreateItem(DropdownItem itemTemplate)
    {
        var item = base.CreateItem(itemTemplate);
        _toggles.Add(item.GetComponentInChildren<Toggle>());
        return item;
    }

    private void ShowMulti()
    {
        _toggles.Clear();
        
        Show();
        
        for (var i = 0; i < _toggles.Count; i++)
        {
            var tog = _toggles[i];

            var index = i; 
            tog.onValueChanged.RemoveAllListeners();
            tog.onValueChanged.AddListener(isOn =>
            {
                m_multiValue[index] = isOn;
                onMultiValueChanged?.Invoke(m_multiValue);
            });

            tog.isOn = m_multiValue[index];
            if (m_multiValue[index])
            {
                tog.Select();
            }
        }
    }
    
    public new void RefreshShownValue() { }
}