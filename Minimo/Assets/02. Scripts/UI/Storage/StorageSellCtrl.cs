using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class StorageSellCtrl : MonoBehaviour
{
    [SerializeField] private Button _increaseBtn;
    [SerializeField] private Button _decreaseBtn;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _sellBtn;
    [SerializeField] private TextMeshProUGUI _priceText;
    
    private StoragePanel _storagePanel;
    private StorageInfoPanel _infoPanel;
    
    private Item _item;
    
    private int _currentCount;
    private string _sellText;

    private float _globalSellCostRatio;

    public void Setup()
    {
        App.GetManager<MinimoManager>()
            .GlobalSellCostRatio
            .Subscribe(value =>
            {
                _globalSellCostRatio = value;
            })
            .AddTo(this);
        
        _storagePanel = App.GetManager<UIManager>().GetPanel<StoragePanel>();
        _infoPanel = App.GetManager<UIManager>().GetPanel<StorageInfoPanel>();
        
        _sellText = App.GetData<TitleData>().GetString("STR_STORAGE_UI_SELL");
        
        _increaseBtn.onClick.AddListener(() => AddCurrentCount(1));
        _decreaseBtn.onClick.AddListener(() => AddCurrentCount(-1));
        _sellBtn.onClick.AddListener(OnClickSell);
    }

    public void Initialize(Item item)
    {
        _item = item;
        
        _currentCount = (item.Count / 2) + 1;
      
        UpdateCurrentCount();
    }
    
    private void UpdateCurrentCount()
    {
        _countText.text = $"X{_currentCount}";

        var price = Mathf.Max(_item.Data.SellCost * _currentCount, 0);
        var modifiedPrice = price * _globalSellCostRatio;
        var roundedPrice = Mathf.RoundToInt(modifiedPrice);
        _priceText.text = string.Format(_sellText, roundedPrice);
        
        Debug.Log($"\u250c\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2510");
        Debug.Log($"\u2502 <color=green>[1] \u25b6</color> <b>기존 판매 가격</b> : {price}");
        Debug.Log($"\u2502 <color=green>[2] \u25b6</color> <b>판매 가격 증가 비율</b> : {_globalSellCostRatio}");
        Debug.Log($"\u2502 <color=green>[3] \u25b6</color> <b>재계산된 가격</b> : {modifiedPrice}");
        Debug.Log($"\u2502 <color=green>[4] \u25b6</color> <b>반올림된 최종 가격</b> : {roundedPrice}");
        Debug.Log($"\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518");
        
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

        if (_currentCount >= _item.Count) 
        {
            _increaseBtn.gameObject.SetActive(false);
        }
        else
        {
            _increaseBtn.gameObject.SetActive(true);
        }
    }

    private void OnClickSell()
    {
        AccountInfo.Instance.RemoveItem(_item.Data.ID, _currentCount);

        _storagePanel.Refresh();
        
        /*
        var newCurrencyRequest = new CurrencyDTO
        {
            Star = _accountInfo.Star.Value,
            BlueStar = _accountInfo.BlueStar.Value + _currentCount * _item.Data.SellCost
        };
        
        _accountInfo.UpdateCurrency(newCurrencyRequest);
        */
        
        _infoPanel.ClosePanel();
    }
}
