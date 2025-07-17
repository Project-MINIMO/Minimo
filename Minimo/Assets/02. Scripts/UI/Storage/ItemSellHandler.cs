using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class ItemSellHandler : TransactionHandler
{
    private Item _item;

    private float _globalSellCostRatio;

    protected override void Awake()
    {
        base.Awake();
        
        App.GetManager<MinimoManager>()
            .GlobalSellCostRatio
            .Subscribe(value =>
            {
                _globalSellCostRatio = value;
            })
            .AddTo(this);

        TransactionString = App.GetData<TitleData>().GetString("STR_STORAGE_UI_SELL");
    }
    
    public void Initialize(Item item)
    {
        _item = item;
        Quantity = (item.Count / 2) + 1;
      
        base.Initialize();
    }
    
    protected override int CalculatePrice()
    {
        var price = Mathf.Max(_item.Data.SellCost * Quantity, 0);
        var modifiedPrice = price * _globalSellCostRatio;
        var roundedPrice = Mathf.RoundToInt(modifiedPrice);
        
        App.LogBox("green", "생산 시간 로그", new()
        {
            { "기존 판매 가격", price.ToString() },
            { "판매 가격 증가 비율", _globalSellCostRatio.ToString() },
            { "재계산된 가격", modifiedPrice.ToString() },
            { "반올림된 최종 가격", roundedPrice.ToString() },
        });

        return roundedPrice;
    }

    protected override int GetMaxQuantity() => _item.Count;
    protected override int GetMinQuantity() => 1;

    protected override void Transaction()
    {
        AccountInfo.Instance.RemoveItem(_item, Quantity);
        AccountInfo.Instance.blueStar += Price;
        
        base.Transaction();
    }
}
