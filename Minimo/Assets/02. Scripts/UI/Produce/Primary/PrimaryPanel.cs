using UnityEngine;
using UnityEngine.UI;

public class PrimaryPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _minimoBtn;
    
    [SerializeField] private PlantCtrl _plantCtrl;
    [SerializeField] private ProduceInfoCtrl _infoCtrl;
    [SerializeField] private HarvestHandler _harvestCtrl;
    
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

        if (_produceManager.CurrentObject.ActiveTask == null)
        {
            if (_produceManager.CurrentObject.AllTasks.Count == 0)
            {
                ShowUI(_plantCtrl);
            }
            else
            {
                ShowUI(_harvestCtrl);
            }
        }
        else
        {
            ShowUI(_infoCtrl);
        }
    }

    private void ShowUI(MonoBehaviour targetUI)
    {
        _harvestCtrl.gameObject.SetActive(targetUI == _harvestCtrl);
        _infoCtrl.SetActive(targetUI == _infoCtrl);
        _plantCtrl.SetActive(targetUI == _plantCtrl);
    }
    
    private void SetPosition()
    {
        var position = _produceManager.CurrentObject.transform.position;
        var screenPos = Camera.main.WorldToScreenPoint(position);
        screenPos.y -= 100;
        _rect.position = screenPos;
    }
}
