using UnityEngine;
using UnityEngine.UI;

public class ProduceSlot : MonoBehaviour
{
    [SerializeField] private Button _taskBtn;

    [SerializeField] private ItemInfoUpdater _itemInfoUpdater;
    [SerializeField] private RemainTimeUpdater _remainTimeUpdater;

    private ProduceManager _produceManager;
    private ProduceTertiary _produceObject;
    private ProduceTask _produceTask;
    
    private int _taskIndex;
    private float _lastUpdateTime;

    private void Awake()
    {
        _taskIndex = transform.GetSiblingIndex();
        
        _taskBtn.onClick.AddListener(OnClickTask);
        
        _produceManager = App.GetManager<ProduceManager>();
    }

    private void OnEnable()
    {
        if (_produceManager == null) return;
        
        _produceObject = _produceManager.CurrentObject as ProduceTertiary;
        _lastUpdateTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - _lastUpdateTime < 0.1f) return;

        _lastUpdateTime = Time.time;

        SetSlot();
    }

    public void SetSlot()
    {
        if (_produceObject == null || _produceObject.AllTasks.Count <= _taskIndex)
        {
            SetEmpty();
            return;
        }
        
        var currentTask = _produceObject.AllTasks[_taskIndex];
        
        if (!ReferenceEquals(_produceTask, currentTask))
        {
            _produceTask = currentTask;
            _itemInfoUpdater.SetItem(_produceTask);
        }
        
        _remainTimeUpdater.SetRemainTime(_produceTask.RemainTime, _produceTask.Data.Time);
    }

    private void SetEmpty()
    {
        _produceTask = null;

        _remainTimeUpdater.SetRemainTime(-1, 1);
        _itemInfoUpdater.SetItemEmpty();
    }
    
    private void OnClickTask()
    {
        if (_produceTask?.CurrentState is PendingState)
        {
            //취소?
        }
        else if (_produceTask?.CurrentState is ActiveState)
        {
            var useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();
            useCashPanel.OpenPanel(UseCashType.Produce, 
                _produceTask.RemainTime, 
                _produceManager.HarvestEarly);
        }
        else if (_produceTask?.CurrentState is CompletedState)
        {
            _produceObject.StartHarvest();
        }
    }
}
