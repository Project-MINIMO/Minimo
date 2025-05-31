using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    
    [SerializeField] private PlantPanel _plantCtrl;
    [SerializeField] private ProduceInfoCtrl _infoCtrl;
    [SerializeField] private HarvestHandler _harvestCtrl;
    
    [SerializeField] private RectTransform _rect;
    
    private ProduceManager _produceManager;

    public override void Initialize()
    {
        _produceManager = App.GetManager<ProduceManager>();

        _closeBtn.onClick.AddListener(() => _produceManager.DeactiveProduce());
    }
 
    public void OpenPanel(ProduceState state)
    {
        base.OpenPanel();

        SetPosition();
        
        switch (state)
        {
            case ProduceState.Idle:
                ShowUI(_plantCtrl);
                break;
            
            case ProduceState.Produce:
                ShowUI(_infoCtrl);
                break;
            
            case ProduceState.Complete:
                ShowUI(_harvestCtrl);
                break;
        }
    }

    private void ShowUI(MonoBehaviour targetUI)
    {
        _harvestCtrl.gameObject.SetActive(targetUI == _harvestCtrl);
        _infoCtrl.SetActive(targetUI == _infoCtrl);
        _plantCtrl.gameObject.SetActive(targetUI == _plantCtrl);
    }
    
    private void SetPosition()
    {
        var position = _produceManager.CurrentProduceObject.transform.position;
        var screenPos = Camera.main.WorldToScreenPoint(position);
        _rect.position = screenPos;
    }
}
