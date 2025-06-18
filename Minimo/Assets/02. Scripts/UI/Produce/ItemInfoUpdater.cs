using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ItemInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _infoTMP;
    [SerializeField] private Image[] _infoImages;
    
    private TitleData _titleData;
    
    private ProduceData _currentOption;
    
    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
    }

    public void SetTaskItem(ProduceTask produceTask)
    {
        _currentOption = produceTask.Data;
        
        SetInfo();
    }
    
    private void SetInfo()
    {
        var i = 0;
        
        for (; i < _currentOption.ResultItems.Length; i++) 
        {
            if (!_titleData.Item.TryGetValue(_currentOption.ResultItems[i].ID, out var itemData))
            {
                Debug.LogError($"Cannot find item data with code : {_currentOption.ResultItems[i].ID}");
                return;
            }
            
            _infoTMP[i].text = $"X{_currentOption.ResultItems[i].Amount}";

            if (_infoImages[i] != null)
            {
                _infoImages[i].sprite = Resources.Load<Sprite>($"Item/{itemData.Name}");
                _infoImages[i].gameObject.SetActive(true);
            }
        }

        for (; i < _infoImages.Length; i++) 
        {
            if (_infoImages[i] != null)
            {
                _infoImages[i]?.gameObject.SetActive(false);
            }
            
        }
    }

    public void SetItemEmpty()
    {
        _infoTMP[0].text = string.Empty;
        _infoImages[0].sprite = null;
    }
}
