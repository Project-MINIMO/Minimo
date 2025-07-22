using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimoSlot : InventorySlot<Minimo>
{
    public override bool CanShow() => true;
    
    [SerializeField] private MinimoInfoUpdater _infoUpdater;

    public override void Initialize(Minimo item)
    {
        base.Initialize(item);

        Item = item;
        
        _infoUpdater.UpdateInfo(item);
        
        Item.OnMinimoLevelChanged += _infoUpdater.UpdateLevelInfo;
        Item.OnAssignedBuildingChanged += _infoUpdater.UpdateAssignedBuilding;
    }
}
