using UniRx;
using UnityEngine;
using TMPro;

public class ProduceInfoCtrl : MonoBehaviour
{
    [SerializeField] private ItemInfoUpdater _itemInfoUpdater;
    [SerializeField] private RemainTimeUpdater _remainTimeUpdater;
    [SerializeField] private TextMeshProUGUI _resultsNameTMP;
    
    private ProduceManager _produceManager;
    private TitleData _titleData;
    private ProduceData _currentOption;
    
    private void Awake()
    {
        _produceManager = App.GetManager<ProduceManager>();
        _titleData = App.GetData<TitleData>();
  
        _produceManager.CurrentRemainTime
            .Subscribe(SetRemainTime)
            .AddTo(gameObject);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);

        if (isActive)
        {
            var currentObject = _produceManager.CurrentProduceObject;
            
            var currentTask = currentObject.ActiveTask;
            _currentOption = currentObject.ActiveTask.Data;
            
            _itemInfoUpdater.SetTaskItem(currentTask);
            _remainTimeUpdater.SetRemainTime(currentTask.RemainTime, _currentOption.Time);

            SetResultsName();
        }
    }

    private void SetRemainTime(int remainTime)
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
    
    private void SetResultsName()
    {
        _resultsNameTMP.text = string.Empty;
        
        var i = 0;
        
        for (; i < _currentOption.ResultItems.Length; i++) 
        {
            if (!_titleData.Item.TryGetValue(_currentOption.ResultItems[i].ID, out var itemData))
            {
                Debug.LogError($"Cannot find item data with code : {_currentOption.ResultItems[i].ID}");
                return;
            }
            
            if (i > 0) 
            {
                _resultsNameTMP.text += " / ";
            }
            
            _resultsNameTMP.text += _titleData.GetString(itemData.Name);
        }
    }
}
