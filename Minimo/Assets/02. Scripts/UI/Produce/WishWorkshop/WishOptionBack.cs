using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class WishOptionBack : MonoBehaviour
{
    private List<WishOptionBtn> _optionBtns;
    
    [SerializeField] private ItemType _itemType;
    
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _buttonParent;
    [SerializeField] private ScrollRect _scrollRect;

    private void Awake()
    {
        var existingButtons = GetComponentsInChildren<WishOptionBtn>(true);

        var filteredItems = App.GetData<TitleData>().Item.Values
            .Where(x => x.Type == (int)_itemType)
            .Where(x => x.Level == 3)
            .ToList();
        _optionBtns = new List<WishOptionBtn>(filteredItems.Count);

        var i = 0;
        
        for (; i < filteredItems.Count; i++)
        {
            var optionBtn = i < existingButtons.Length 
                ? existingButtons[i] 
                : Instantiate(_buttonPrefab, _buttonParent).GetComponent<WishOptionBtn>();

            optionBtn.Initialize(filteredItems[i].ID);
            _optionBtns.Add(optionBtn);
            
            optionBtn.gameObject.SetActive(false);
            optionBtn.gameObject.SetActive(true);
        }

        for (; i < existingButtons.Length; i++)
        {
            existingButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        foreach (var button in _optionBtns)
        {
            button.gameObject.SetActive(true);
        }
        
        _scrollRect.verticalNormalizedPosition = 1;
    }
}
