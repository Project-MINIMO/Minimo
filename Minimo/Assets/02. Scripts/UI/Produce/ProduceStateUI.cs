using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ProduceStateUI : MonoBehaviour
{
    [SerializeField] private GameObject _stateBack;
    [SerializeField] private GameObject _idleObj;
    [SerializeField] private GameObject _completeObj;
    [SerializeField] private Image _completeImg;
    
    private ProduceObject _produceObject;
    
    private void Start()
    {
        _produceObject = GetComponentInParent<ProduceObject>();
        if (_produceObject == null)
        {
            Debug.LogWarning("ProduceObject not found in parent.");
            return;
        }
        if (_produceObject.BuildingData.Type == 0) return;
        
        _produceObject.OnProduceStateChanged += UpdateStateUI;
        UpdateStateUI(_produceObject.CurrentState);
        
        var editManager = App.GetManager<EditManager>();
        editManager.IsBuildingEditing
            .Subscribe(isEditing =>
            {
                _stateBack.SetActive(!isEditing);
            }).AddTo(gameObject);
        editManager.IsTileEditing
            .Subscribe(isEditing =>
            {
                _stateBack.SetActive(!isEditing);
            }).AddTo(gameObject);
        gameObject.SetActive(!editManager.IsBuildingEditing.Value);
    }

    private void UpdateStateUI(ProduceState state)
    {
        switch (state)
        {
            case ProduceState.Idle:
                _idleObj.SetActive(true);
                _completeObj.SetActive(false);
                break;
            
            case ProduceState.Produce:
                _idleObj.SetActive(false);
                _completeObj.SetActive(false);
                break;
            
            case ProduceState.Complete:
                _idleObj.SetActive(false);
                _completeObj.SetActive(true);
                var completeTask = _produceObject.AllTasks
                    .FirstOrDefault(x => x.CurrentState is CompletedState);
                if (completeTask != null)
                {
                    var item = AccountInfo.Instance.Items[completeTask.Data.ResultItems[0].ID];
                    _completeImg.sprite = item.Icon;
                }
                break;
        }
    }
}
