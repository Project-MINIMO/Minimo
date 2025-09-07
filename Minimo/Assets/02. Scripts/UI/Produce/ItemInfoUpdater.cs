#nullable enable
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI? _itemNameTMP;
    [SerializeField] private TextMeshProUGUI? _itemDescriptionTMP;
    [SerializeField] private TextMeshProUGUI? _itemCountTMP;
    [SerializeField] private Image? _iconImg;
    
    public void ClearItem()
    {
        _itemNameTMP?.SetText(string.Empty);
        _itemCountTMP?.SetText(string.Empty);
        _itemDescriptionTMP?.SetText(string.Empty);
        if (_iconImg == null) return; 
        _iconImg.gameObject.SetActive(false);
    }
    
    public void UpdateItem(int itemId, int amount)
    {
        var itemData = AccountInfo.Instance.Items[itemId];
        UpdateItem(itemData, amount);
    }

    public void UpdateItem(Item item, int amount)
    {
        _itemNameTMP?.SetText(item.Name);
        _itemDescriptionTMP?.SetText(item.Description);
        UpdateItem(item.Icon, amount);
    }
    
    public void UpdateItem(Sprite sprite, int amount)
    {
        UpdateItem(sprite, amount.ToString());
    }

    public void UpdateItem(Sprite sprite, string amount)
    {
        _itemCountTMP?.SetText(amount);

        if (_iconImg == null) return;
        _iconImg.sprite = sprite;
        _iconImg.gameObject.SetActive(true);
    }

    public void UpdateItem(int itemId)
    {
        var item = AccountInfo.Instance.Items[itemId];
        UpdateItem(item);
    }
    
    public void UpdateItem(Item item)
    {
        _itemNameTMP?.SetText(item.Name);
        _itemDescriptionTMP?.SetText(item.Description);
        UpdateItem(item.Icon, item.Count);
    }

    public void UpdateItemCount(int amount)
    {
        _itemCountTMP?.SetText(amount.ToString());
    }
}
