using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _infoTMP;
    [SerializeField] private Image _infoImage;
    
    private TitleData _titleData;
    
    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
    }

    public void SetItem(ProduceTask produceTask)
    {
        var item = produceTask.Data.ResultItems[0];
        
        if (!_titleData.Item.TryGetValue(item.ID, out var itemData))
        {
            Debug.LogError($"Cannot find item data with code : {item.ID}");
            return;
        }

        if (_infoTMP)
        {
            _infoTMP.text = _titleData.GetString($"STR_ITEM_{itemData.Name.ToUpper()}_NAME");
        }

        if (_infoImage)
        {
            _infoImage.sprite = Resources.Load<Sprite>($"Item/{itemData.Name}");
            _infoImage.gameObject.SetActive(true);
        }
    }
    
    public void SetItemEmpty()
    {
        if (_infoTMP)
        {
            _infoTMP.text = string.Empty;
        }

        if (_infoImage)
        {
            _infoImage.sprite = null;
        }
    }
}
