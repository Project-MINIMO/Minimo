using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private HarvestHandler _harvestCtrl;
    [SerializeField] private ProduceInfoCtrl _infoCtrl;
    [SerializeField] private RectTransform _rect;
    
    private ProduceManager _produceManager;
    private PlantPanel _plantPanel;

    public override void Initialize()
    {
        _produceManager = App.GetManager<ProduceManager>();
        
        _produceManager.IsProducing
            .Subscribe((isProducing) =>
            {
                if (!isProducing)
                {
                    ClosePanel();
                    return;
                }
                
                if (true/*_produceManager.CurrentProduceObject.IsPrimary*/)
                {
                    OpenPanel();
                }
                else
                {
                    ClosePanel();
                }
            }).AddTo(gameObject);

        _plantPanel = App.GetManager<UIManager>().GetPanel<PlantPanel>();
        _closeBtn.onClick.AddListener(() =>
        {
            _produceManager.DeactiveProduce();
        });
    }
 
    public override void OpenPanel()
    {
        base.OpenPanel();

        SetPosition();
        
        var currentObject = _produceManager.CurrentProduceObject;
        var currentIndex = currentObject.AllTasks.IndexOf(currentObject.ActiveTask);
        if (currentIndex > 0 || (currentObject.ActiveTask is null && currentObject.AllTasks.Count > 0))
        {
            ShowUI(_harvestCtrl);
        }
        else if (currentIndex == 0)
        {
            ShowUI(_infoCtrl);
        }
        else
        {
            _plantPanel.OpenPanel();
            ShowUI(_plantPanel);
        }
    }

    private void ShowUI(MonoBehaviour targetUI)
    {
        _harvestCtrl.gameObject.SetActive(targetUI == _harvestCtrl);
        _infoCtrl.SetActive(targetUI == _infoCtrl);
    }
    
    private void SetPosition()
    {
        var position = _produceManager.CurrentProduceObject.transform.position;
        var screenPos = Camera.main.WorldToScreenPoint(position);
        _rect.position = screenPos;
    }
}
