using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemNameTMP;
    [SerializeField] private TextMeshProUGUI _itemCountTMP;
    [SerializeField] private Image _iconImg;
    
    private TitleData _titleData;
    
    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
    }

    public void SetItem(ProduceTask produceTask)
    {
        SetItem(produceTask.Data.ResultItems[0]);
    }

    public void SetItem(ProduceData data)
    {
        SetItem(data.ResultItems[0]);
    }

    private void SetItem(ProduceResult result)
    {
        if (!_titleData.Item.TryGetValue(result.ID, out var itemData))
        {
            Debug.LogError($"Cannot find item data with code : {result.ID}");
            return;
        }

        if (_itemNameTMP)
        {
            _itemNameTMP.text = _titleData.GetString($"STR_ITEM_{itemData.Name.ToUpper()}_NAME");
        }

        if (_itemCountTMP)
        {
            _itemCountTMP.text = result.Amount.ToString();
        }

        if (_iconImg)
        {
            _iconImg.sprite = Resources.Load<Sprite>($"Item/{itemData.Name}");
            _iconImg.gameObject.SetActive(true);
        }
    }
    
    public void SetItemEmpty()
    {
        if (_itemNameTMP)
        {
            _itemNameTMP.text = string.Empty;
        }
        
        if (_itemCountTMP)
        {
            _itemCountTMP.text = string.Empty;
        }

        if (_iconImg)
        {
            _iconImg.sprite = null;
        }
    }
}
