#nullable enable
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimoInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI? _nameTMP;
    [SerializeField] private TextMeshProUGUI? _descriptionTMP;
    
    [SerializeField] private TextMeshProUGUI? _levelTMP;
    [SerializeField] private GameObject[]? _starObjs;
    [SerializeField] private TextMeshProUGUI[]? _abilityTMPS;
    
    [SerializeField] private Image? _assignedBuildingImg;
    
    private Minimo _minimo;
    
    public void UpdateInfo(Minimo minimo)
    {
        _minimo = minimo;
        
        _nameTMP?.SetText(minimo.Name);
        _descriptionTMP?.SetText(minimo.Description);
        
        UpdateLevelInfo(minimo, minimo.Level);
        UpdateAssignedBuilding(minimo.AssignedBuilding);
    }
    
    public void UpdateLevelInfo(Minimo minimo, int level)
    {
        _levelTMP?.SetText($"Lv.{level}");
        
        var starCount = level / 10 + 1;
        for (var i = 0; i < _starObjs?.Length; i++)
        {
            _starObjs[i].SetActive(i < starCount);
        }
        
        for (var i = 0; i < _abilityTMPS?.Length; i++)
        {
            _abilityTMPS[i].SetText(string.Format(_minimo.AbilityDescriptions[i], _minimo.Abilities[i].Value));
        }
    }
    
    public void UpdateAssignedBuilding(ProduceAdvanced building)
    {
        if (_assignedBuildingImg == null) return;
        
        if (building == null)
        {
            _assignedBuildingImg.gameObject.SetActive(false);
        }
        else
        {
            _assignedBuildingImg.gameObject.SetActive(true);
            _assignedBuildingImg.sprite = building.BuildingData.Icon;
        }
    }
}
