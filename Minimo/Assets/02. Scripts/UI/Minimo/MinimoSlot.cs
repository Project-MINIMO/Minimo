using UnityEngine;

public class MinimoSlot : InventorySlot<Minimo>
{
    public override bool CanShow() => true;
    
    [SerializeField] private MinimoInfoUpdater _info;

    public override void Initialize(Minimo item)
    {
        base.Initialize(item);

        Item = item;
        _info.UpdateInfo(item);
    }
}
