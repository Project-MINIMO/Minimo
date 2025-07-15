#nullable enable
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemNameTMP;
    [SerializeField] private TextMeshProUGUI _itemCountTMP;
    [SerializeField] private Image _iconImg;

    public void UpdateItem(ProduceResult? result)
    {
        if (result is null)
        {
            _itemNameTMP?.SetText(string.Empty);
            _itemCountTMP?.SetText(string.Empty);
            if (_iconImg) _iconImg.gameObject.SetActive(false);
        }
        else
        {
            var itemData = AccountInfo.Instance.Items[result.ID];
            _itemNameTMP?.SetText(itemData.Name);
            _itemCountTMP?.SetText(result.Amount.ToString());
            
            if (_iconImg)
            {
                _iconImg.sprite = itemData.Icon;
                _iconImg.gameObject.SetActive(true);
            }
        }
    }
}
