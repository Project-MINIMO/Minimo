using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ProduceTaskBtn : MonoBehaviour
{
    public bool IsActive => gameObject.activeSelf;
    
    [SerializeField] private Button _taskBtn;
    [SerializeField] private TextMeshProUGUI _taskText;
    
    [SerializeField] private ItemInfoUpdater _itemInfoUpdater;
    [SerializeField] private RemainTimeUpdater _remainTimeUpdater;

    private int _currentRemainTime; 
    private PlantCtrl _plantPanel;

    private ProduceManager _produceManager;
    private ProduceObject _produceObject;
    private ProduceTask _produceTask;
    private int _taskIndex;

    private void Start()
    {
        _taskIndex = transform.GetSiblingIndex();
        _plantPanel = App.GetManager<UIManager>().GetPanel<PlantCtrl>();
        
        _taskBtn.onClick.AddListener(OnClickTask);
        
        _produceManager = App.GetManager<ProduceManager>();
  
        _produceManager.CurrentRemainTime
            .Subscribe(SetRemainTime)
            .AddTo(gameObject);
    }
    
    public void Initialize(ProduceObject produceObject)
    {
        _produceObject = produceObject;
    }
    
    private void Update()
    {
        if (gameObject.activeSelf == false) 
        {
            return;
        }
        
        if (_produceObject == null)
        {
            return;
        }

        if (_produceObject.AllTasks.Count <= _taskIndex) 
        {
            _remainTimeUpdater.SetFillAmount(0);
            _remainTimeUpdater.SetRemainText(TaskState.Empty);
            _itemInfoUpdater.SetItemEmpty();
            _produceTask = null;
            return;
        }
        
        var tempTask = _produceObject.AllTasks[_taskIndex];
        
        if (!Equals(_produceTask, tempTask))
        {
            _produceTask = tempTask;
            _itemInfoUpdater.SetTaskItem(_produceTask);
            SetRemainTime(_produceTask.RemainTime);
        }
    }
    
    private async void OnClickTask()
    {
        if (_produceTask == null) 
        {
            _plantPanel.OpenPanel();
            return;
        }
            
        if (_produceTask?.CurrentState is PendingState)
        {
            //취소?
        }
        else if (_produceTask?.CurrentState is ActiveState)
        {
            var useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();
            var currentStarCount = (_produceTask.RemainTime / 30) + 1;
            useCashPanel.OpenPanel(UseCashType.Produce, 
                currentStarCount, 
                ()=>_produceManager.HarvestEarly());
        }
        else if (_produceTask?.CurrentState is CompletedState)
        {
            _produceManager.CurrentProduceObject.StartHarvest();
        }
    }
    
    private void SetRemainTime(int remainTime)
    {
        if (remainTime < 0)
        {
            _remainTimeUpdater.SetFillAmount(1);
            _remainTimeUpdater.SetRemainText(TaskState.Complete);
            return;
        }
        
        if (_produceTask?.CurrentState is PendingState)
        {
            _remainTimeUpdater.SetFillAmount(0);
            _remainTimeUpdater.SetRemainText(TaskState.Pending);
        }
        else if (_produceTask?.CurrentState is ActiveState)
        {
            _remainTimeUpdater.SetRemainTime(remainTime, _produceTask.Data.Time);
        }
        else if (_produceTask?.CurrentState is CompletedState)
        {
            _remainTimeUpdater.SetFillAmount(1);
            _remainTimeUpdater.SetRemainText(TaskState.Complete);
        }
        else
        {
            _remainTimeUpdater.SetFillAmount(0);
            _remainTimeUpdater.SetRemainText(TaskState.Empty);
            _itemInfoUpdater.SetItemEmpty();
        }
    }
}
