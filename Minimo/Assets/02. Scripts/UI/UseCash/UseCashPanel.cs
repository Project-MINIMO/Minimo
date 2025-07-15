using System;
using System.Collections.Generic;

using UnityEngine;
using UniRx;

public enum UseCashType
{
    Produce,
    ProduceExpand,
    ProduceMaterial,
}

public class UseCashPanel : UIBase
{
    [SerializeField] private UseCashBack _useBack;
    [SerializeField] private UseCashMaterialBack _useMaterialBack;
    
    [SerializeField] private ChargeCashBack _chargeBack;

    private TitleData _titleData;
    private string _diamondCountString;
    
    private Action _useAction;
    private int _useCount;

    private float _globalTimeSkipCostRatio;
    
    public override void Initialize()
    {
        _titleData = App.GetData<TitleData>();

        _useBack.Initialize(_titleData, ClosePanel, OpenCharge);
        _useMaterialBack.Initialize(_titleData, ClosePanel, OpenCharge);
        _chargeBack.Initialize(_titleData, ClosePanel);
        
        App.GetManager<MinimoManager>()
            .GlobalTimeSkipCostRatio
            .Subscribe(value =>
            {
                _globalTimeSkipCostRatio = value;
            })
            .AddTo(this);
    }

    public void OpenPanel(UseCashType type, float amount, Action useAction)
    {
        base.OpenPanel();

        ActiveBacks(isActiveUse: true);
        
        _useBack.gameObject.SetActive(true);
        var price = (int)(amount / _titleData.Common["TimeSkipCost"] + 1);
        var modifiedPrice = _globalTimeSkipCostRatio * price;
        var roundedPrice = Mathf.RoundToInt(modifiedPrice);
        _useBack.Setup(type, roundedPrice, useAction);
        
        Debug.Log($"\u250c\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2510");
        Debug.Log($"\u2502 <color=blue>[1] \u25b6</color> <b>기존 시간 단축 비용</b> : {price}");
        Debug.Log($"\u2502 <color=blue>[2] \u25b6</color> <b>시간 단축 비율</b> : {_globalTimeSkipCostRatio}");
        Debug.Log($"\u2502 <color=blue>[3] \u25b6</color> <b>재계산된 비용</b> : {modifiedPrice}");
        Debug.Log($"\u2502 <color=blue>[4] \u25b6</color> <b>반올림된 최종 비용</b> : {roundedPrice}");
        Debug.Log($"\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518");
    }

    public void OpenPanel(List<(Item, int)> lackItems, Action useAction)
    {
        base.OpenPanel();
        
        ActiveBacks(isActiveUse: true);
        
        _useMaterialBack.gameObject.SetActive(true);
        _useMaterialBack.Setup(lackItems, useAction);
    }
    
    private void OpenCharge()
    {
        ActiveBacks(isActiveUse: false);
    }
    
    private void ActiveBacks(bool isActiveUse)
    {
        _useBack.gameObject.SetActive(false);
        _useMaterialBack.gameObject.SetActive(false);
        _chargeBack.gameObject.SetActive(!isActiveUse);
    }
}
