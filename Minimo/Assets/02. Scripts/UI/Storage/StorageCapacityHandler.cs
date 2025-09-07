using System;

public class StorageCapacityHandler : CapacityHandler
{
    protected override int GetExpandCost(TitleData title) => title.Common["StorageExpandCost"];
    protected override int GetCurrentCapacity() => AccountInfo.Instance.StorageCapacity;
    protected override int GetBaseCapacity() => 20;

    protected override void SuccessTransaction()
    {
        AccountInfo.Instance.AddStorageCapacity(Quantity - CurrentCapacity);
    }
}
