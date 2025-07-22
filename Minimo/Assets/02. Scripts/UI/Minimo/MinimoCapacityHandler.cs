using System;

public class MinimoCapacityHandler : CapacityHandler
{
    protected override void Awake()
    {
        base.Awake();
        
        var titleData = App.GetData<TitleData>();
        ExpandCost = titleData.Common["ResidenceExpandCost"];
        TransactionString = titleData.GetString("STR_STORAGE_EXPAND_COST");
    }
    
    public override void Initialize(Action transactionCallback)
    {
        CurrentCapacity = AccountInfo.Instance.MinimoCapacity;
        BaseCapacity = 20;
        
        base.Initialize(transactionCallback);
        
        Initialize();
    }

    protected override void SuccessTransaction()
    {
        AccountInfo.Instance.AddMinimoCapacity(Quantity - CurrentCapacity);
    }
}