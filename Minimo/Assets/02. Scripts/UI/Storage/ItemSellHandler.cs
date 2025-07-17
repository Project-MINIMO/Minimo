using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class ItemSellHandler : MonoBehaviour
{
    [SerializeField] private Button _increaseBtn;
    [SerializeField] private Button _decreaseBtn;
    [SerializeField] private Button _sellBtn;
        
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private TextMeshProUGUI _priceText;

    private Item _item;
    
    private int _amount;
    private int _price;
    
    private string _sellText;
    private float _globalSellCostRatio;

    private void Awake()
    {
        App.GetManager<MinimoManager>()
            .GlobalSellCostRatio
            .Subscribe(value =>
            {
                _globalSellCostRatio = value;
            })
            .AddTo(this);

        _sellText = App.GetData<TitleData>().GetString("STR_STORAGE_UI_SELL");
        
        _increaseBtn.onClick.AddListener(() => ChangeAmount(1));
        _decreaseBtn.onClick.AddListener(() => ChangeAmount(-1));
        _sellBtn.onClick.AddListener(OnClickSell);
    }
    
    private void ChangeAmount(int amount)
    {
        _amount += amount;
        UpdatePrice();
    }
    
    private void OnClickSell()
    {
        AccountInfo.Instance.RemoveItem(_item, _amount);
        AccountInfo.Instance.blueStar += _price;
    }

    public void SetItem(Item item)
    {
        _item = item;
        
        _amount = (item.Count / 2) + 1;
      
        UpdatePrice();
    }
    
    private void UpdatePrice()
    {
        _amountText.SetText($"X{_amount}");

        var price = Mathf.Max(_item.Data.SellCost * _amount, 0);
        var modifiedPrice = price * _globalSellCostRatio;
        _price = Mathf.RoundToInt(modifiedPrice);
        _priceText.text = string.Format(_sellText, _price);
        
        App.LogBox("green", "생산 시간 로그", new()
        {
            { "기존 판매 가격", price.ToString() },
            { "판매 가격 증가 비율", _globalSellCostRatio.ToString() },
            { "재계산된 가격", modifiedPrice.ToString() },
            { "반올림된 최종 가격", _price.ToString() },
        });

        UpdateVisibility();
    }
    
    private void UpdateVisibility()
    {
        _increaseBtn.gameObject.SetActive(_amount < _item.Count);
        _decreaseBtn.gameObject.SetActive(_amount > 1);
    }
}
