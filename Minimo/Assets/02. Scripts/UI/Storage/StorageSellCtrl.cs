using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageSellCtrl : MonoBehaviour
{
    [SerializeField] private Button _increaseBtn;
    [SerializeField] private Button _decreaseBtn;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _sellBtn;
    [SerializeField] private TextMeshProUGUI _priceText;
    
    private StoragePanel _storagePanel;
    private StorageInfoPanel _infoPanel;
    
    private ItemData _item;
    
    private int _currentCount;
    private string _sellText;

    public void Setup()
    {
        _storagePanel = App.GetManager<UIManager>().GetPanel<StoragePanel>();
        _infoPanel = App.GetManager<UIManager>().GetPanel<StorageInfoPanel>();
        
        _sellText = App.GetData<TitleData>().GetString("STR_STORAGE_UI_SELL");
        
        _increaseBtn.onClick.AddListener(() => AddCurrentCount(1));
        _decreaseBtn.onClick.AddListener(() => AddCurrentCount(-1));
        _sellBtn.onClick.AddListener(OnClickSell);
    }
    
    public void Initialize(ItemData item)
    {
        _item = item;
        
        if (AccountInfo.Instance.items.TryGetValue(item, out var value))
        {
            _currentCount = (value / 2) + 1;
        }
        else
        {
            _currentCount = 0;
        }
        UpdateCurrentCount();
    }
    
    private void UpdateCurrentCount()
    {
        _countText.text = $"X{_currentCount}";
        _priceText.text = string.Format(_sellText, Mathf.Max(_item.SellCost * _currentCount, 0));
        
        UpdateButtonActive();
    }
    
    private void AddCurrentCount(int amount)
    {
        _currentCount += amount;
        
        UpdateCurrentCount();
    }
    
    private void UpdateButtonActive()
    {
        if (_currentCount <= 1) 
        {
            _decreaseBtn.gameObject.SetActive(false);
        }
        else
        {
            _decreaseBtn.gameObject.SetActive(true);
        }

        if (AccountInfo.Instance.items.TryGetValue(_item, out var value))
        {
            if (_currentCount >= value) 
            {
                _increaseBtn.gameObject.SetActive(false);
            }
            else
            {
                _increaseBtn.gameObject.SetActive(true);
            }
        }
    }

    private void OnClickSell()
    {
        if (AccountInfo.Instance.items.ContainsKey(_item))
        {
            AccountInfo.Instance.items[_item] += -_currentCount;
        }
        
        /*
        var newCurrencyRequest = new CurrencyDTO
        {
            Star = _accountInfo.Star.Value,
            BlueStar = _accountInfo.BlueStar.Value + _currentCount * _item.Data.SellCost
        };
        
        _accountInfo.UpdateCurrency(newCurrencyRequest);
        */
        
        _storagePanel.OnStorageChanged();
        _infoPanel.ClosePanel();
    }
}
