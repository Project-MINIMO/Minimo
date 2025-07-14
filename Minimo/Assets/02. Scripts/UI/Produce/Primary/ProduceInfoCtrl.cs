using UnityEngine;

public class ProduceInfoCtrl : MonoBehaviour
{
    [SerializeField] private ItemInfoUpdater _itemInfoUpdater;
    [SerializeField] private RemainTimeUpdater _remainTimeUpdater;
    
    private ProduceManager _produceManager;
    private ProduceTask _produceTask;
    private ProduceData _currentOption;
    
    private void Awake()
    {
        _produceManager = App.GetManager<ProduceManager>();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);

        if (isActive)
        {
            var currentObject = _produceManager.CurrentProduceObject;
            
            _produceTask = currentObject.ActiveTask;
            _currentOption = currentObject.ActiveTask.Data;

            _produceTask.OnRemainTimeChanged += SetRemainTime;
            
            _itemInfoUpdater.SetItem(_produceTask);
        }
        else
        {
            _produceTask.OnRemainTimeChanged -= SetRemainTime;
        }
    }

    private void SetRemainTime(float remainTime)
    {
        if (_currentOption == null)
        {
            return;
        }

        if (remainTime <= 0)
        {
            gameObject.SetActive(false);
        }
        
        _remainTimeUpdater.SetRemainTime(remainTime, _currentOption.Time);
    }
}
