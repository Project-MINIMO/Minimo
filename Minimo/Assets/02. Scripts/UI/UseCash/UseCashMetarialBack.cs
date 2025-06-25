using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class UseCashMaterialBack : UseCashBack
{
    [SerializeField] private CashMaterialSlot[] _materialSlots;
    
    public void Setup(List<(ItemData, int)> lackItems, Action useAction)
    {
        var price = CalculatePrice(lackItems);
        
        base.Setup(UseCashType.ProduceMaterial, price, useAction);
        
        SetMaterialSlots(lackItems);
    }

    private int CalculatePrice(List<(ItemData, int)> lackItems)
    {
        return 0; //lackItems.Sum(lackItem => lackItem.Item1.Data.CashCost * lackItem.Item2);
    }

    private void SetMaterialSlots(List<(ItemData, int)> lackItems)
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
