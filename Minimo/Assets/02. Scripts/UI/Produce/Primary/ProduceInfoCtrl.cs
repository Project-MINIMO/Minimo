using UnityEngine;

public class ProduceInfoCtrl : MonoBehaviour
{
    [SerializeField] private ItemInfoUpdater _itemInfoUpdater;
    [SerializeField] private RemainTimeUpdater _remainTimeUpdater;
    
    private ProduceManager _produceManager;
    private ProduceTask _produceTask;
    
    private void Awake()
    {
        _produceManager = App.GetManager<ProduceManager>();
    }

    private void OnEnable()
    {
        if (_produceManager == null) return;
        
        _produceTask = _produceManager.CurrentObject.ActiveTask;
        _produceTask.OnRemainTimeChanged += SetRemainTime;
        SetRemainTime(_produceTask.RemainTime);
        _itemInfoUpdater.UpdateItem(_produceTask.Data.ResultItems[0]);
    }

    private void OnDisable()
    {
        if (_produceTask == null) return;
            
        _produceTask.OnRemainTimeChanged -= SetRemainTime;
        _produceTask = null;
    }

    private void SetRemainTime(float remainTime)
    {
        if (_produceTask == null)
        {
            return;
        }

        _remainTimeUpdater.UpdateTime(remainTime, _produceTask.Data.Time);
    }
}
