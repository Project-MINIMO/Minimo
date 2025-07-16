using UnityEngine;
using UnityEngine.UI;

public class ProduceSlot : MonoBehaviour
{
    [SerializeField] private Button _slotBtn;
    [SerializeField] private GameObject[] _stateImgs;
    
    [SerializeField] private ItemInfoUpdater _itemInfoUpdater;
    [SerializeField] private RemainTimeUpdater _remainTimeUpdater;
    [SerializeField] private GameObject _infoBack;
    [SerializeField] private Button _infoCloseBtn;
    
    private ProduceManager _produceManager;
    private ProduceTertiary _produceObject;
    private ProduceTask _produceTask;
    
    private int _taskIndex;

    private void Awake()
    {
        _produceManager = App.GetManager<ProduceManager>();
        _taskIndex = transform.GetSiblingIndex();
        
        _slotBtn.onClick.AddListener(OnClickSlot);
        _infoCloseBtn.onClick.AddListener((() => _infoBack.SetActive(false)));
    }

    private void OnEnable()
    {
        _infoBack.SetActive(false);
        
        if (_produceManager == null) return;
        
        _produceObject = _produceManager.CurrentObject as ProduceTertiary;
        if (_produceObject == null) return;
        
        _produceObject.OnTaskCountChanged += UpdateSlots;
        UpdateSlots(_produceObject.AllTasks.Count);
    }

    private void OnDisable()
    {
        _infoBack.SetActive(false);
        
        if (_produceObject == null) return;

        UnbindTask();
        
        _produceObject.OnTaskCountChanged -= UpdateSlots;
        _produceObject = null;
    }

    private void UpdateSlots(int taskCount)
    {
        if (taskCount > _taskIndex)
        {
            BindTask(_produceObject.AllTasks[_taskIndex]);
        }
        else
        {
            UnbindTask();
        }
    }
    
    private void BindTask(ProduceTask task)
    {
        if (_produceTask != null)
        {
            _produceTask.OnRemainTimeChanged -= OnRemainTimeChanged;
            _produceTask.OnStateChanged -= OnStateChanged;
        }
        
        _produceTask = task;
        _produceTask.OnRemainTimeChanged += OnRemainTimeChanged;
        _produceTask.OnStateChanged += OnStateChanged;

        _itemInfoUpdater.UpdateItem(_produceTask.Result);
        OnRemainTimeChanged(_produceTask.RemainTime);
        OnStateChanged(_produceTask.CurrentState);
    }

    private void UnbindTask()
    {
        if (_produceTask != null)
        {
            _produceTask.OnRemainTimeChanged -= OnRemainTimeChanged;
            _produceTask.OnStateChanged -= OnStateChanged;
            _produceTask = null;
        }
        
        _itemInfoUpdater.UpdateItem(null);
        _remainTimeUpdater.UpdateTime(-1, 1);
        OnStateChanged(PendingState.Instance);
    }

    private void OnRemainTimeChanged(float remain)
    {
        if (_produceTask == null) return;
        
        _remainTimeUpdater.UpdateTime(remain, _produceTask.ModifiedTime);
    }

    private void OnStateChanged(ITaskState state)
    {
        _infoBack.SetActive(false);
        
        _stateImgs[0].SetActive(state is ActiveState);
        _stateImgs[1].SetActive(state is CompletedState);
        
        OnRemainTimeChanged(_produceTask.RemainTime);
    }

    private void OnClickSlot()
    {
        switch (_produceTask?.CurrentState)
        {
            case PendingState:
                //취소?
                break;
            
            case ActiveState:
                _infoBack.SetActive(true);
                break;
            
            case CompletedState:
                _produceManager.Harvest();
                break;
        }
    }
}
