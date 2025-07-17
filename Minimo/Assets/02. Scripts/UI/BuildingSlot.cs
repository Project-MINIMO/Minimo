using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingSlot : InventorySlot<Building>
{
    public override bool CanShow() => Item.IsLocked;
    
    [SerializeField] private Image _buildingImg;
    [SerializeField] private TextMeshProUGUI _buildingNameTMP;
    [SerializeField] private GameObject _lockBack;
    [SerializeField] private TextMeshProUGUI _lockTMP;
    
    private void OnEnable()
    {
        SetState();
    }
        
    public override void Initialize(Building item)
    {
        base.Initialize(item);

        Item = item;
        _buildingImg.sprite = item.Icon;
        _buildingNameTMP.SetText(item.Name);

        _lockTMP.SetText(App.GetData<TitleData>().GetFormatString("STR_BUILDING_UI_LOCK", item.UnlockLevel.ToString()));
    }

    private void SetState()
    {
        if (Item == null) return;
        
        _lockBack.SetActive(CanShow());
    }
}
