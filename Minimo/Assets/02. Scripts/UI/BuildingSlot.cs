using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingSlot : InventorySlot<Building>
{
    public override bool CanShow() => true;
    
    [SerializeField] private Image _buildingImg;
    [SerializeField] private TextMeshProUGUI _buildingNameTMP;
    
    public override void Initialize(Building item)
    {
        base.Initialize(item);

        Item = item;
        _buildingImg.sprite = item.Icon;
        _buildingNameTMP.SetText(item.Name);
    }
}
