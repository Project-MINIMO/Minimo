using UnityEngine;
using UnityEngine.UI;

public class SecondaryPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _minimoBtn;
    
    [SerializeField] private GameObject[] _stateCtrls;
    
    [SerializeField] private RectTransform _rect;
    
    private ProduceManager _produceManager;
    private ProduceObject _produceObject;
    private MinimoPlacePanel _placePanel;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _produceManager = App.GetManager<ProduceManager>();
        _placePanel = manager.GetPanel<MinimoPlacePanel>();

        _closeBtn.onClick.AddListener(_produceManager.Deselect);
        _minimoBtn.onClick.AddListener(() => 
            _placePanel.OpenPanel(_produceManager.CurrentObject as ProduceAdvanced));
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        SetPosition();

        _produceObject = _produceManager.CurrentObject;
        _produceObject.OnProduceStateChanged += ShowCtrls;
        
        ShowCtrls(_produceManager.CurrentObject.CurrentState);
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
}