using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimoShiftWindow : PopUpWindow
{
    public override PopUpType Type() => PopUpType.MinimoShift;
    
    [SerializeField] private MinimoInfoUpdater _beforeInfoUpdater;
    [SerializeField] private MinimoInfoUpdater _afterInfoUpdater;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;

    private ProduceAdvanced _currentProduce;
    private Minimo _currentMinimo;
    
    protected override void Awake()
    {
        base.Awake();

        _descriptionTMP.text = App.GetData<TitleData>().GetString("STR_POPUP_PLACE_DESC1");
        AssignBtn.GetComponentInChildren<TextMeshProUGUI>().text = App.GetData<TitleData>().GetString("STR_BUTTON_CONFIRM");
    }

    public override void Show(ProduceAdvanced building, Minimo minimo)
    {
        base.Show();

        _currentProduce = building;
        _currentMinimo = minimo;
        
        _beforeInfoUpdater.UpdateInfo(_currentProduce.AssignedMinimo);
        _afterInfoUpdater.UpdateInfo(minimo);
    }

    protected override void Assign()
    {
        _currentProduce.PlaceMinimo(_currentMinimo);
        
        base.Assign();
    }
}
