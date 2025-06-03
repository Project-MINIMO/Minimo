using System.Linq;

using UnityEngine;

public class ProduceStateUI : MonoBehaviour
{
    [SerializeField] private GameObject _idleBack;
    [SerializeField] private GameObject _produceBack;
    [SerializeField] private GameObject _completeBack;

    private EditManager _editManager;
    private ProduceObject _object;
    private float _lastUpdateTime;
    
    private void Start()
    {
        _editManager = App.GetManager<EditManager>();
        _object = GetComponent<ProduceObject>();
        _lastUpdateTime = Time.time;

        if (_object.BuildingData.Type == 0)
        {
            enabled = false;
        }
    }
    
    private void Update()
    {
        if (Time.time - _lastUpdateTime < 0.1f) return;

        _lastUpdateTime = Time.time;
        
        if (_editManager.IsEditing.Value)
        {
            _idleBack.SetActive(false);
            _produceBack.SetActive(false);
            _completeBack.SetActive(false);
            return;
        }

        var state = GetCurrentProduceState();
        
        UpdateStateUI(state);
    }
    
    private ProduceState GetCurrentProduceState()
    {
        if (_object.AllTasks.Any(x => x.CurrentState is CompletedState))
        {
            return ProduceState.Complete;
        }

        return _object.ActiveTask != null 
            ? ProduceState.Produce 
            : ProduceState.Idle;
    }
    
    private void UpdateStateUI(ProduceState state)
    {
        if (state == ProduceState.Complete && _object.BuildingData.Type is 1)
        {
            _idleBack.SetActive(false);
            _produceBack.SetActive(false);
            _completeBack.SetActive(true);
            return;
        }
        
        _idleBack.SetActive(state == ProduceState.Idle);
        _produceBack.SetActive(state == ProduceState.Produce);
        _completeBack.SetActive(state == ProduceState.Complete);
    }
}
