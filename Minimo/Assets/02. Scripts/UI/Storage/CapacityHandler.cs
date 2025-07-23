using System;

using UnityEngine;
using TMPro;

public abstract class CapacityHandler : TransactionHandler
{
    [SerializeField] private TextMeshProUGUI _currentCapacityTMP;
    
    protected override int Step => 10;
    
    private UseCashPanel _useCashPanel;

    private Action _transactionAction;
    protected int ExpandCost;
    protected int CurrentCapacity;
    protected int BaseCapacity;
    
    protected override void Awake()
    {
        base.Awake();
        
        _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();
    }

    public virtual void Initialize(Action transactionCallback)
    {
        _transactionAction = transactionCallback;
        Quantity = CurrentCapacity + 10;
        _currentCapacityTMP.SetText($"{BaseCapacity}/{CurrentCapacity}");
        
        Initialize();
    }
    
    protected override int CalculatePrice() => Mathf.Max(ExpandCost * (Quantity - CurrentCapacity), 0);

    protected override int GetMaxQuantity() => 9999; //TODO : 레벨별로 다른 최대용량
    protected override int GetMinQuantity() => CurrentCapacity + 10;

    protected override void Transaction()
    {
        if (Price <= AccountInfo.Instance.Cash)
        {
            AccountInfo.Instance.Cash -= Price;
            
            _transactionAction?.Invoke();
        }
        else
        {
            _useCashPanel.OpenPanel();
        }
    }
    
    protected abstract void SuccessTransaction();
}
