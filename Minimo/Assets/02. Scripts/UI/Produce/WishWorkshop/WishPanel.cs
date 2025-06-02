using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WishPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;
    
    private ProduceTaskBtn[] _taskBtns;
    [SerializeField] private Button _expandBtn;

    private ProduceManager _produceManager;
    
    public override void Initialize()
    {
        _produceManager = App.GetManager<ProduceManager>();
        
        _taskBtns = GetComponentsInChildren<ProduceTaskBtn>(true);
        _closeBtn.onClick.AddListener(()=> _produceManager.DeactiveProduce());
        _expandBtn.onClick.AddListener(() =>
        {
            ((ProduceTertiary)_produceManager.CurrentProduceObject).AddSlotCount();
            InitializeTaskBtns();
        });
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _titleTMP.text = App.GetData<TitleData>()
            .GetString($"STR_BUILDING_{_produceManager.CurrentProduceObject.BuildingData.Name.ToUpper()}_NAME");

        InitializeTaskBtns();
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