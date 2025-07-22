using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimoSlot : InventorySlot<Minimo>
{
    public override bool CanShow() => true;
    
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _levelTMP;
    [SerializeField] private GameObject[] _starObjs;
    [SerializeField] private GameObject _assignedBuildingBack;
    [SerializeField] private Image _assignedBuildingImg;
    
    public override void Initialize(Minimo item)
    {
        base.Initialize(item);

        Item = item;
        
        _nameTMP.SetText(item.Name);
        
        Item.OnMinimoLevelChanged += UpdateLevelInfo;
        UpdateLevelInfo(item.Level);

        Item.OnAssignedBuildingChanged += UpdateAssignedBuilding;
        UpdateAssignedBuilding(item.AssignedBuilding);
    }
    
    private void UpdateLevelInfo(int level)
    {
        _levelTMP.text = $"Lv.{level}";
        var starCount = level / 10 + 1;
        for (var i = 0; i < _starObjs.Length; i++)
        {
            _starObjs[i].SetActive(i < starCount);
        }
    }
    
    private void UpdateAssignedBuilding(ProduceAdvanced building)
    {
        if (building == null)
        {
            _assignedBuildingBack.SetActive(false);
        }
        else
        {
            _assignedBuildingBack.SetActive(true);
            _assignedBuildingImg.sprite = building.BuildingData.Icon;
        }
    }
}
