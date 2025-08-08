using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimoAssignedSlot : InventorySlot<ProduceAdvanced>
{
    public override bool CanShow() => true;
    
    [SerializeField] private Image _buildingImg;
    [SerializeField] private TextMeshProUGUI _buildingNameTMP;
    [SerializeField] private MinimoInfoUpdater _minimoInfoUpdater;
    [SerializeField] private GameObject _emptyObj;
  
    public override void Initialize(ProduceAdvanced item)
    {
        base.Initialize(item);

        Item = item;
        item.OnMinimoAssigned += OnMinimoAssigned;
        OnMinimoAssigned(null);
        
        _buildingImg.sprite = item.BuildingData.Icon;
        _buildingNameTMP.SetText(item.BuildingData.Name);
    }

    private void OnMinimoAssigned(Minimo minimo)
    {
        if (minimo == null)
        {
            _emptyObj.SetActive(true);
            _minimoInfoUpdater.gameObject.SetActive(false);
        }
        else
        {
            _emptyObj.SetActive(false);
            _minimoInfoUpdater.gameObject.SetActive(true);
            _minimoInfoUpdater.UpdateInfo(minimo);
        }
    }
}
