using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StorageInventory : Inventory<Item>
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private InventoryDropdown _dropdown;
    
    private List<InventorySlot<Item>> _activeSlots;
    
    private void OnEnable()
    {
        _menuTogs[0].isOn = true;
    }

    protected override void SetString()
    {
        var titleData = App.GetData<TitleData>();
        _menuTogs[0].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB1_NAME");
        _menuTogs[1].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB2_NAME");
        _menuTogs[2].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB3_NAME");
        _menuTogs[3].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB4_NAME");
        _menuTogs[4].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB5_NAME");
    }

    protected override void FilterSlots(int index)
    {
        base.FilterSlots(index);
        
        SortDefault();
        
        _activeSlots = Slots.Where(slot => slot.gameObject.activeSelf).ToList();
        _dropdown.OnMenuChanged(index);
    }

    protected override List<Item> GetFilteredItems() => AccountInfo.Instance.Items.Values.ToList();

    protected override bool IsSlotFiltered(int index, Item item) => index == 0 || index == (int)item.Type + 1;

    #region Sort
    public void SortDefault()
    {
        Debug.Log("기본순 정렬");
        var sorted = _activeSlots.OrderBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByCount(bool desc)    
    {
        Debug.Log($"개수 {(desc?"↑":"↓")}");
        
        var sorted = desc
            ? _activeSlots.OrderBy(slot => slot.Item.Count).ThenBy(slot => slot.Item.ID).ToList()
            : _activeSlots.OrderByDescending(slot => slot.Item.Count).ThenBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    public void SortByPrice(bool desc)
    {
        Debug.Log($"가격 {(desc ? "↑" : "↓")}");

        var sorted = desc
            ? _activeSlots.OrderBy(slot => slot.Item.SellCost).ThenBy(slot => slot.Item.ID).ToList()
            : _activeSlots.OrderByDescending(slot => slot.Item.SellCost).ThenBy(slot => slot.Item.ID).ToList();

        SortSlots(sorted);
    }
    private void SortSlots(List<InventorySlot<Item>> slots)
    {
        for (var i = 0; i < slots.Count; i++)
        {
            slots[i].transform.SetSiblingIndex(i);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
    }
    #endregion

    #region Filter
    public void FilterItem(int level)
    {
        Debug.Log($"식품 필터: {level}");
        
        foreach (var slot in _activeSlots)
        {
            var isActive = level == 0 || slot.Item.Level == level;
            
            slot.gameObject.SetActive(isActive);
        }
    }
    public void FilterProps(ItemProperty property)
    {
        Debug.Log($"기물 필터: {property}");
        
        foreach (var slot in _activeSlots)
        {
            var isActive = property == 0 || slot.Item.Property == property;
            
            slot.gameObject.SetActive(isActive);
        }
    }
    #endregion
}
