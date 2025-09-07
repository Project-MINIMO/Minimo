using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UseCashPanel : UIBase
{
    public override bool IsUseBlur => true;
    
    [SerializeField] private UseCashMaterialBack _useMaterialBack;
    [SerializeField] private ChargeCashBack _chargeBack;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _useMaterialBack.Initialize(ClosePanel, () => ActiveBacks(false));
        _chargeBack.Initialize(ClosePanel);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        ActiveBacks(false);
    }

    public void OpenPanel(List<(Item, int)> lackItems, Action onConfirm, Action onCancel)
    {
        base.OpenPanel();
        
        ActiveBacks(true);
        
        _useMaterialBack.Setup(lackItems, onConfirm, onCancel);
    }

    private void ActiveBacks(bool isActiveUse)
    {
        _useMaterialBack.gameObject.SetActive(isActiveUse);
        _chargeBack.gameObject.SetActive(!isActiveUse);
    }
}
