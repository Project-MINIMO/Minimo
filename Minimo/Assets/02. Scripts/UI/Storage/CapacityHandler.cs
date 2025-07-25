using System;

using UnityEngine;
using TMPro;

public abstract class CapacityHandler : TransactionHandler
{
    [SerializeField] private TextMeshProUGUI _currentCapacityTMP;
    
    protected override int Step => 10;
    protected int CurrentCapacity;

    private Action _transactionAction;
    private int _expandCost;
    private int _baseCapacity;
    
    protected override void Awake()
    {
        base.Awake();
      
        var titleData = App.GetData<TitleData>();
        _expandCost = GetExpandCost(titleData);
        TransactionString = titleData.GetString("STR_STORAGE_EXPAND_COST");
    }
    
    protected abstract int GetExpandCost(TitleData title);

    public void Initialize(Action transactionCallback)
    {
        _transactionAction = transactionCallback;
        
        CurrentCapacity = GetCurrentCapacity();
        Quantity = CurrentCapacity + 10;
        _currentCapacityTMP.SetText($"{GetBaseCapacity()}/{CurrentCapacity}");
        
        Initialize();
    }
    
    protected abstract int GetCurrentCapacity();
    protected abstract int GetBaseCapacity();
    
    protected override int CalculatePrice() => Mathf.Max(_expandCost * (Quantity - CurrentCapacity), 0);
    protected override int GetMaxQuantity() => 9999; //TODO : 레벨별로 다른 최대용량
    protected override int GetMinQuantity() => CurrentCapacity + 10;

    protected override void Transaction()
    {
        if (Price <= AccountInfo.Instance.Gold.Count)
        {
            AccountInfo.Instance.Gold.AddCount(-Price);
            SuccessTransaction();
            _transactionAction?.Invoke();
        }
        else
        {
            App.Notification(NotifyType.GoldLack);
        }
    }

    protected abstract void SuccessTransaction();
}
