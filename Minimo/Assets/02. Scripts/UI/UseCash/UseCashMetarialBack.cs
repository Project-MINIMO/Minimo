using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class UseCashMaterialBack : UseCashBack
{
    [SerializeField] private CashMaterialSlot[] _materialSlots;
    
    public void Setup(List<(Item, int)> lackItems, Action useAction)
    {
        var price = CalculatePrice(lackItems);
        
        base.Setup(UseCashType.ProduceMaterial, price, useAction);
        
        SetMaterialSlots(lackItems);
    }

    private int CalculatePrice(List<(Item, int)> lackItems)
    {
        return lackItems.Sum(lackItem => lackItem.Item1.Data.BuyCost * lackItem.Item2);
    }

    private void SetMaterialSlots(List<(Item, int)> lackItems)
    {
        var i = 0;
        
        for (; i < lackItems.Count; i++) 
        {
            _materialSlots[i].SetData(lackItems[i].Item1, lackItems[i].Item2);
        }

        for (; i < _materialSlots.Length; i++) 
        {
            _materialSlots[i].SetNull();
        }
    }
}
