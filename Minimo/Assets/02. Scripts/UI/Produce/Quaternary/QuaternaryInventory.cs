using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class QuaternaryInventory : MonoBehaviour
{
    private List<WishOptionBtn> _optionBtns;

    [SerializeField] private Toggle[] _menuTogs;
    [SerializeField] private ScrollRect _scrollRect;

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
    }

    private void InitSlots()
    {
        var existingButtons = GetComponentsInChildren<WishOptionBtn>(true);

        var filteredItems = App.GetData<TitleData>().Item.Values
            .Where(x => x.Level == 3)
            .ToList();
        _optionBtns = new List<WishOptionBtn>(filteredItems.Count);
        
        var i = 0;
        
        for (; i < filteredItems.Count; i++)
        {
            var optionBtn = existingButtons[i];

            optionBtn.Initialize(filteredItems[i].ID);
            _optionBtns.Add(optionBtn);

            optionBtn.gameObject.SetActive(true);
        }

        for (; i < existingButtons.Length; i++)
        {
            existingButtons[i].gameObject.SetActive(false);
        }
    }

    public void Show(int index)
    {
        gameObject.SetActive(true);
        FilterStorageBtns(index);
    }
    
    private void FilterStorageBtns(int index)
    {
        foreach (var button in _optionBtns)
        {
            var isActive = index == button.Item.Data.Type - 1;
            
            button.gameObject.SetActive(isActive);
        }
        
        _scrollRect.verticalNormalizedPosition = 1;
    }
}
