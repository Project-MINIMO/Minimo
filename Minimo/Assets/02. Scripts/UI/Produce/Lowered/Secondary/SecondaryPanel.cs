using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecondaryPanel : UIBase
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _placeMinimoBtn;
    
    [SerializeField] private UILongPressDetector _longPressDetector;
    [SerializeField] private GameObject _minimoInfoObj;
    [SerializeField] private TextMeshProUGUI _minimoInfoTMP;
    [SerializeField] private GameObject _minimoImg;
    
    [SerializeField] private GameObject[] _stateCtrls;
    
    [SerializeField] private RectTransform _rect;
    
    private ProduceManager _produceManager;
    private ProduceSecondary _produceObject;
    private PlaceByBuildingPanel _placePanel;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _produceManager = App.GetManager<ProduceManager>();
        _placePanel = manager.GetPanel<PlaceByBuildingPanel>();

        _longPressDetector.OnLongPress += () =>
        {
            if (_produceObject.AssignedMinimo == null) return;
            _minimoInfoTMP.SetText(GetMinimoInfo());
            _minimoInfoObj.SetActive(true);
        };
        _longPressDetector.OnClickUp += () => _minimoInfoObj.SetActive(false);
        
        _closeBtn.onClick.AddListener(_produceManager.Deselect);
        _placeMinimoBtn.onClick.AddListener(() => 
            _placePanel.OpenPanel(_produceManager.CurrentObject as ProduceAdvanced));
    }

    public override void OpenPanel()
    {
        _titleTMP.SetText(_produceManager.CurrentObject.BuildingData.Name);
        
        SetPosition();
        _minimoInfoObj.SetActive(false);

        _produceObject = _produceManager.CurrentObject as ProduceSecondary;
        if (_produceObject == null) return;
        _produceObject.OnProduceStateChanged += ShowCtrls;
        
        ShowCtrls(_produceManager.CurrentObject.CurrentState);
        
        base.OpenPanel();
    }

    public override void ClosePanel()
    {
        foreach (var ctrl in _stateCtrls)
        {
            ctrl.SetActive(false);
        }

        if (_produceObject != null)
        {
            _produceObject.OnProduceStateChanged -= ShowCtrls;
            _produceObject = null;
        }
        
        base.ClosePanel();
    }
    
    public override void Show(bool isNew)
    {
        base.Show(isNew);
        
        _minimoImg.SetActive(_produceObject.AssignedMinimo != null);
        _minimoInfoObj.SetActive(false);
    }

    private void ShowCtrls(ProduceState state)
    {
        if (_produceManager == null) return;
        if (_produceManager.CurrentObject == null) return;
        
        for (var i = 0; i < _stateCtrls.Length; i++)
        {
            _stateCtrls[i].SetActive(i == (int)state);
        }

        if (state == ProduceState.Complete)
        {
            _produceManager.Deselect();
        }
    }

    private void SetPosition()
    {
        var position = _produceManager.CurrentObject.transform.position;
        var screenPos = Camera.main.WorldToScreenPoint(position);
        screenPos.y -= 100;
        _rect.position = screenPos;
    }
    
    private string GetMinimoInfo()
    {
        var info = string.Empty;
        var minimo = _produceObject.AssignedMinimo;
        for (var i = 0; i < minimo.Abilities.Count; i++)
        {
            if (i > 0) info += "\n";
            info += string.Format(minimo.AbilityDescriptions[i], minimo.Abilities[i].Value);
        }

        return info;
    }
}