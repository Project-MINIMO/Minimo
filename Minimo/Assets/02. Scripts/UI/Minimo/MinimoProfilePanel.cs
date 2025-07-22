using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinimoProfilePanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;

    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _levelTMP;
    [SerializeField] private GameObject[] _starObjs;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private Button _assignButton;
    [SerializeField] private Image _assignedBuildingImg;
    [SerializeField] private Button _levelUpBtn;
    [SerializeField] private TextMeshProUGUI[] _abilityTMPS;

    [SerializeField] private Toggle[] _menuTogs;
    [SerializeField] private GameObject[] _menuBacks;
    
    private Minimo _currentMinimo;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        for (var i = 0; i < _menuTogs.Length; i++)
        {
            var index = i;
            
            _menuTogs[index].onValueChanged.AddListener(isOn => 
                _menuBacks[index].gameObject.SetActive(isOn));
        }

        _levelUpBtn.onClick.AddListener(() => _currentMinimo?.AddLevel(1));
        _closeBtn.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel(Minimo minimo)
    {
        OpenPanel();
        _currentMinimo = minimo;
        _nameTMP.SetText(minimo.Name);
        _descriptionTMP.SetText(minimo.Description);
        
        minimo.OnMinimoLevelChanged += UpdateLevelInfo;
        UpdateLevelInfo(minimo.Level);
        
        minimo.OnAssignedBuildingChanged += UpdateAssignedBuilding;
        UpdateAssignedBuilding(minimo.AssignedBuilding);

        for (var i = 0; i < _abilityTMPS.Length; i++)
        {
            _abilityTMPS[i].SetText(string.Format(minimo.AbilityDescriptions[i], minimo.Abilities[i].Value));
        }
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        if (_currentMinimo == null) return;
        _currentMinimo.OnMinimoLevelChanged -= UpdateLevelInfo;
        _currentMinimo.OnAssignedBuildingChanged -= UpdateAssignedBuilding;
    }
    
    private void UpdateLevelInfo(int level)
    {
        _levelTMP.text = $"Lv.{level}";
        var starCount = level / 10 + 1;
        for (var i = 0; i < _starObjs.Length; i++)
        {
            _starObjs[i].SetActive(i < starCount);
        }
        
        for (var i = 0; i < _abilityTMPS.Length; i++)
        {
            _abilityTMPS[i].SetText(string.Format(_currentMinimo.AbilityDescriptions[i], _currentMinimo.Abilities[i].Value));
        }
    }
    
    private void UpdateAssignedBuilding(ProduceAdvanced building)
    {
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
