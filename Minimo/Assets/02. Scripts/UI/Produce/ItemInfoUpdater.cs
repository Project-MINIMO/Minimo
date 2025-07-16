#nullable enable
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemNameTMP;
    [SerializeField] private TextMeshProUGUI _itemCountTMP;
    [SerializeField] private Image _iconImg;

    public void UpdateItem(int itemId, int amount)
    {
        if (itemId == -1)
        {
            _itemNameTMP?.SetText(string.Empty);
            _itemCountTMP?.SetText(string.Empty);
            if (_iconImg) _iconImg.gameObject.SetActive(false);
        }
        else
        {
            var itemData = AccountInfo.Instance.Items[itemId];
            UpdateItem(itemData, amount);
        }
    }

    public void UpdateItem(Item item, int amount)
    {
        _itemNameTMP?.SetText(item.Name);
        _itemCountTMP?.SetText(amount.ToString());

        if (!_iconImg) return;
        _iconImg.sprite = item.Icon;
        _iconImg.gameObject.SetActive(true);
    }
}
