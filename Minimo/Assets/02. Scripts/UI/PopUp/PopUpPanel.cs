using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PopUpPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    
    private Dictionary<PopUpType, PopUpWindow> _popUpMap = new();
    private PopUpWindow _currentWindow;
    
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

    public override void ClosePanel()
    {
        _currentWindow?.Hide();
        _currentWindow = null;
        
        base.ClosePanel();
    }
}
