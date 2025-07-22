using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimoProfilePanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private Toggle[] _menuTogs;
    [SerializeField] private GameObject[] _menuBacks;
    
    [SerializeField] private MinimoInfoUpdater _infoUpdater;

    [SerializeField] private Button _assignButton;
    [SerializeField] private Button _levelUpBtn;
    
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
        _infoUpdater.UpdateInfo(minimo);
        
        minimo.OnMinimoLevelChanged += _infoUpdater.UpdateLevelInfo;
        minimo.OnAssignedBuildingChanged += _infoUpdater.UpdateAssignedBuilding;
        
        _menuTogs[1].isOn = true;
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        if (_currentMinimo == null) return;
        _currentMinimo.OnMinimoLevelChanged -= _infoUpdater.UpdateLevelInfo;
        _currentMinimo.OnAssignedBuildingChanged -= _infoUpdater.UpdateAssignedBuilding;
    }
}
