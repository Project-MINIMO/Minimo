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
    [SerializeField] private Button _historyBtn;
    
    private Minimo _currentMinimo;
    private string _historyString;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        var titleData = App.GetData<TitleData>();
        _titleTMP.SetText(titleData.GetString("STR_MINIMOPROFILE_NAME"));
        _levelUpBtn.GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_MINIMOPROFILE_LEVELUP");
        _historyString = titleData.GetString("STR_MINIMOPROFILE_HISTORY");
        
        for (var i = 0; i < _menuTogs.Length; i++)
        {
            var index = i;
            
            _menuBacks[index].gameObject.SetActive(true);
            _menuBacks[index].gameObject.SetActive(false);
            _menuTogs[index].onValueChanged.AddListener(isOn => 
                _menuBacks[index].gameObject.SetActive(isOn));
            _menuTogs[index].GetComponentInChildren<TextMeshProUGUI>().text = GetMenuString(index, titleData);
        }

        _levelUpBtn.onClick.AddListener(() => _currentMinimo?.AddLevel(1));
        _closeBtn.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel(Minimo minimo)
    {
        OpenPanel();
        
        _currentMinimo = minimo;
        _infoUpdater.UpdateInfo(minimo);
        _historyBtn.GetComponentInChildren<TextMeshProUGUI>().text = string.Format(_historyString, minimo.Name);
        
        minimo.OnLevelChanged += _infoUpdater.UpdateLevelInfo;
        minimo.OnAssignmentChanged += _infoUpdater.UpdateAssignedBuilding;
        
        _menuTogs[1].isOn = true;
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        if (_currentMinimo == null) return;
        _currentMinimo.OnLevelChanged -= _infoUpdater.UpdateLevelInfo;
        _currentMinimo.OnAssignmentChanged -= _infoUpdater.UpdateAssignedBuilding;
    }

    private string GetMenuString(int index, TitleData title) => index switch
    {
        0 => title.GetString("STR_MINIMOPROFILE_TAB1_NAME"),
        1 => title.GetString("STR_MINIMOPROFILE_TAB2_NAME"),
        2 => title.GetString("STR_MINIMOPROFILE_TAB3_NAME"),
        _ => string.Empty
    };
}
