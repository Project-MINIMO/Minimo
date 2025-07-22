using System;

public class StorageCapacityHandler : CapacityHandler
{
    protected override void Awake()
    {
        base.Awake();
        
        var titleData = App.GetData<TitleData>();
        ExpandCost = titleData.Common["StorageExpandCost"];
        TransactionString = titleData.GetString("STR_STORAGE_EXPAND_COST");
    }
    
    public override void Initialize(Action transactionCallback)
    {
        CurrentCapacity = AccountInfo.Instance.StorageCapacity;
        BaseCapacity = AccountInfo.Instance.CurrentItemCounts;
        
        base.Initialize(transactionCallback);
        
        Initialize();
    }

    protected override void SuccessTransaction()
    {
        AccountInfo.Instance.AddStorageCapacity(Quantity - CurrentCapacity);
    }
}
