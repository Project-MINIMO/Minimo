using UnityEngine;
using UniRx;

public class ProduceStateUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _stateUIs;

    private EditManager _editManager;
    private ProduceObject _object;
    private float _lastUpdateTime;

    private void Start()
    {
        _editManager = App.GetManager<EditManager>();
        _object = GetComponentInParent<ProduceObject>();
        
        _editManager.IsEditing
            .Subscribe((isEditing) =>
            {
                gameObject.SetActive(!isEditing);
            }).AddTo(gameObject);
        
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
        
        UpdateStateUI(_object.CurrentState);
    }
    
    private void UpdateStateUI(ProduceState state)
    {
        for (var i = 0; i < _stateUIs.Length; i++)
        {
            _stateUIs[i].SetActive(i == (int)state);
        }
    }
}
