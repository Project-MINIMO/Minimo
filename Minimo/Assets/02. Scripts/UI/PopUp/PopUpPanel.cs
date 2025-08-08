using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PopUpPanel : UIBase
{
    public override bool IsUseBlur => _isUseBlur;
    
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _closeImg;
    
    private Dictionary<PopUpType, PopUpWindow> _popUpMap = new();
    private PopUpWindow _currentWindow;
    
    private bool _isUseBlur = true;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _closeBtn.onClick.AddListener(ClosePanel);

        var windows = GetComponentsInChildren<PopUpWindow>(true);
        foreach (var window in windows)
        {
            window.gameObject.SetActive(true);
            _popUpMap.Add(window.Type(), window);
            window.OnAssign += ClosePanel;
            window.gameObject.SetActive(false);
        }
    }

    public void OpenPanel(PopUpType type)
    {
        _isUseBlur = type != PopUpType.StrayWarning;
        _background.color = type != PopUpType.StrayWarning ? Color.white : Color.clear;
        _closeImg.SetActive(type != PopUpType.StrayWarning);
        
        OpenPanel();
        
        _currentWindow = _popUpMap[type];
        _currentWindow.Show();
    }
    
    public void OpenPanel(PopUpType type, ProduceAdvanced building, Minimo minimo)
    {
        OpenPanel();
        
        _currentWindow = _popUpMap[type];
        _currentWindow.Show(building, minimo);
    }

    public void OpenPanel(PopUpType type, MinimoAcquireHandler handler)
    {
        OpenPanel();
        
        _currentWindow = _popUpMap[type];
        _currentWindow.Show(handler);
    }

    public override void ClosePanel()
    {
        _currentWindow?.Hide();
        _currentWindow = null;
        
        base.ClosePanel();
    }
}
