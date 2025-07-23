using System;
using UnityEngine;

public class MinimoCapacityHandler : CapacityHandler
{
    protected override int GetExpandCost(TitleData title) => title.Common["ResidenceExpandCost"];
    protected override int GetCurrentCapacity() => AccountInfo.Instance.MinimoCapacity;
    protected override int GetBaseCapacity() => 20;

    protected override void SuccessTransaction()
    {
        AccountInfo.Instance.AddMinimoCapacity(Quantity - CurrentCapacity);
    }
}