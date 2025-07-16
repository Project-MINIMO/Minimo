using UnityEngine;
using UnityEngine.UI;

public class PrimaryPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _minimoBtn;
    
    [SerializeField] private GameObject[] _stateCtrls;
    
    [SerializeField] private RectTransform _rect;
    
    private ProduceManager _produceManager;
    private PlaceMinimoPanel _placeMinimoPanel;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _produceManager = App.GetManager<ProduceManager>();
        _placeMinimoPanel = manager.GetPanel<PlaceMinimoPanel>();

        _closeBtn.onClick.AddListener(_produceManager.Deselect);
        _minimoBtn.onClick.AddListener(_placeMinimoPanel.OpenPanel);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        SetPosition();

        ShowCtrls(null);
    }

    public override void ClosePanel()
    {
        foreach (var ctrl in _stateCtrls)
        {
            ctrl.SetActive(false);
        }

        if (_produceManager.CurrentObject.ActiveTask != null)
        {
            _produceManager.CurrentObject.ActiveTask.OnStateChanged -= ShowCtrls;
        }
        
        base.ClosePanel();
    }

    private void ShowCtrls(ITaskState taskState)
    {
        if (_produceManager == null) return;
        if (_produceManager.CurrentObject == null) return;
        
        var state = _produceManager.CurrentObject.CurrentState;
        
        for (var i = 0; i < _stateCtrls.Length; i++)
        {
            _stateCtrls[i].SetActive(i == (int)state);
        }

        switch (state)
        {
            case ProduceState.Produce:
                _produceManager.CurrentObject.ActiveTask.OnStateChanged += ShowCtrls;
                break;
            
            case ProduceState.Complete:
                _produceManager.CurrentObject.AllTasks[0].OnStateChanged -= ShowCtrls;
                break;
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
