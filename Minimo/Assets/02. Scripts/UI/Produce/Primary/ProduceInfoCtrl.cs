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
        _produceTask.OnRemainTimeChanged += OnRemainTimeChanged;
        _itemInfoUpdater.UpdateItem(_produceTask.Result);
        OnRemainTimeChanged(_produceTask.RemainTime);
    }

    private void OnDisable()
    {
        if (_produceTask == null) return;
        
        _produceTask.OnRemainTimeChanged -= OnRemainTimeChanged;
        _produceTask = null;
    }

    private void OnRemainTimeChanged(float remainTime)
    {
        if (_produceTask == null)
        {
            return;
        }

        _remainTimeUpdater.UpdateTime(remainTime, _produceTask.ModifiedTime);
    }
}
