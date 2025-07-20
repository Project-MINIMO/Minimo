using System;

using UnityEngine;
using TMPro;

public class CapacityHandler : TransactionHandler
{
    [SerializeField] private TextMeshProUGUI _currentCapacityTMP;
    
    protected override int Step => 10;
    
    private UseCashPanel _useCashPanel;

    private Action _transactionAction;
    private int _expandCost;
    private int _currentCapacity;
    
    protected override void Awake()
    {
        base.Awake();
        
        _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();
        
        var titleData = App.GetData<TitleData>();
        _expandCost = titleData.Common["StorageExpandCost"];
        TransactionString = titleData.GetString("STR_STORAGE_EXPAND_COST");
    }

    public void Initialize(Action transactionCallback)
    {
        _transactionAction = transactionCallback;
        _currentCapacity = AccountInfo.Instance.Capacity;
        Quantity = _currentCapacity + 10;
        _currentCapacityTMP.SetText($"{AccountInfo.Instance.CurrentItemCounts}/{_currentCapacity}");
        
        Initialize();
    }
    
    protected override int CalculatePrice() => Mathf.Max(_expandCost * (Quantity - _currentCapacity), 0);

    protected override int GetMaxQuantity() => 9999; //TODO : 레벨별로 다른 최대용량
    protected override int GetMinQuantity() => _currentCapacity + 10;

    protected override void Transaction()
    {
        if (Price <= AccountInfo.Instance.Cash)
        {
            AccountInfo.Instance.Cash -= Price;
            AccountInfo.Instance.AddCapacity(Quantity - _currentCapacity);
            
            _transactionAction?.Invoke();
        }
        else
        {
            _useCashPanel.OpenPanel();
        }
    }
}
