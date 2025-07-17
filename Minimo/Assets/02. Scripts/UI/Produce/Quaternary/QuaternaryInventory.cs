using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class QuaternaryInventory : MonoBehaviour
{
    [SerializeField] private Toggle[] _menuTogs;
    [SerializeField] private ScrollRect _scrollRect;

    private List<InventorySlot> _slots;
    
    private void Awake()
    {
        InitSlots();
        
        for (var i = 0; i < _menuTogs.Length; i++)
        {
            var index = i;
            _menuTogs[index].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    FilterStorageBtns(index);
                }
            });
        }
        
        FilterStorageBtns(0);
    }

    private void InitSlots()
    {
        var existingButtons = _scrollRect.GetComponentsInChildren<InventorySlot>(true);

        var filteredItems = App.GetData<TitleData>().Item.Values
            .Where(x => x.Level == 3)
            .ToList();
        _slots = new List<InventorySlot>(filteredItems.Count);
        
        var i = 0;
        
        for (; i < filteredItems.Count; i++)
        {
            var slot = existingButtons[i];

            slot.Initialize(filteredItems[i].ID);
            _slots.Add(slot);

            slot.gameObject.SetActive(true);
        }

        for (; i < existingButtons.Length; i++)
        {
            existingButtons[i].gameObject.SetActive(false);
        }
    }

    private void FilterStorageBtns(int index)
    {
        foreach (var button in _slots)
        {
            var isActive = index == button.Item.Data.Type - 1;
            
            button.gameObject.SetActive(isActive);
        }
        
        _scrollRect.verticalNormalizedPosition = 1;
    }
}
