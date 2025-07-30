using System;
using UnityEngine;

public class MinimoSlot : InventorySlot<Minimo>
{
    public override bool CanShow() => true;
    
    [SerializeField] private MinimoInfoUpdater _infoUpdater;

    public override void Initialize(Minimo item)
    {
        base.Initialize(item);

        Item = item;
        
        _infoUpdater.UpdateInfo(item);
        
        Item.OnLevelChanged += _infoUpdater.UpdateLevelInfo;
        Item.OnAssignmentChanged += _infoUpdater.UpdateAssignedBuilding;
    }

    private void OnEnable()
    {
        if (Item.AcquisitionDate == DateTime.MinValue) gameObject.SetActive(false);
    }
}
