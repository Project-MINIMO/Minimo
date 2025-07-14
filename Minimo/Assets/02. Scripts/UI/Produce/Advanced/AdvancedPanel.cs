using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdvancedPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;
    
    private ProduceSlot[] _taskBtns;
    [SerializeField] private Button _expandBtn;
    [SerializeField] private PlantCtrl _plantCtrl;
    
    [SerializeField] private Button _placeMinimoBtn;

    private ProduceManager _produceManager;
    private PlaceMinimoPanel _placeMinimoPanel;
    
    public override void Initialize()
    {
        _produceManager = App.GetManager<ProduceManager>();
        _placeMinimoPanel = App.GetManager<UIManager>().GetPanel<PlaceMinimoPanel>();
        
        _taskBtns = GetComponentsInChildren<ProduceSlot>(true);
        _closeBtn.onClick.AddListener(()=> _produceManager.DeactiveProduce());
        _expandBtn.onClick.AddListener(() =>
        {
            ((ProduceTertiary)_produceManager.CurrentProduceObject).AddSlotCount();
            InitializeTaskBtns();
        });
        _placeMinimoBtn.onClick.AddListener(_placeMinimoPanel.OpenPanel);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _titleTMP.text = App.GetData<TitleData>()
            .GetString($"STR_BUILDING_{_produceManager.CurrentProduceObject.BuildingData.Name.ToUpper()}_NAME");

        InitializeTaskBtns();
        _plantCtrl.SetActive(true);
    }

    private void InitializeTaskBtns()
    {
        var maxCount = ((ProduceTertiary)_produceManager.CurrentProduceObject).MaxSlotCount;
        var i = 0;

        for (; i < maxCount; i++)
        {
            _taskBtns[i].gameObject.SetActive(true);
            _taskBtns[i].SetSlot();
        }

        for (; i < _taskBtns.Length; i++)
        {
            _taskBtns[i].gameObject.SetActive(false);
        }
        
        _expandBtn.gameObject.SetActive(maxCount < 5);
    }
}
