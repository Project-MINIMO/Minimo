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

    private float _globalSellCostRatio;
    
    public override void Initialize()
    {
        _titleData = App.GetData<TitleData>();

        _useBack.Initialize(_titleData, ClosePanel, OpenCharge);
        _useMaterialBack.Initialize(_titleData, ClosePanel, OpenCharge);
        _chargeBack.Initialize(_titleData, ClosePanel);
        
        App.GetManager<MinimoManager>()
            .GlobalHarvestRatio
            .Subscribe(value =>
            {
                _globalSellCostRatio = value;
            })
            .AddTo(this);
    }

    public void OpenPanel(UseCashType type, float amount, Action useAction)
    {
        base.OpenPanel();

        ActiveBacks(isActiveUse: true);
        
        _useBack.gameObject.SetActive(true);
        var price = amount / _titleData.Common["TimeSkipCost"] + 1;
        price *= _globalSellCostRatio;
        var roundedPrice = Mathf.RoundToInt(price);
        _useBack.Setup(type, roundedPrice, useAction);
    }

    public void OpenPanel(List<(ItemData, int)> lackItems, Action useAction)
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
