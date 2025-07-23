using UnityEngine;
using UniRx;

public class ProduceStateUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _stateUIs;
    
    private void Start()
    {
        var editManager = App.GetManager<EditManager>();
        editManager.IsBuildingEditing
            .Subscribe(isEditing =>
            {
                gameObject.SetActive(!isEditing);
            }).AddTo(gameObject);
        editManager.IsTileEditing
            .Subscribe(isEditing =>
            {
                gameObject.SetActive(!isEditing);
            }).AddTo(gameObject);
        
        var obj = GetComponentInParent<ProduceObject>();
        if (obj == null)
        {
            Debug.LogWarning("ProduceObject not found in parent.");
            return;
        }
        if (obj.BuildingData.Type == 0) return;
        
        obj.OnProduceStateChanged += UpdateStateUI;
    }

    private void UpdateStateUI(ProduceState state)
    {
        for (var i = 0; i < _stateUIs.Length; i++)
        {
            _stateUIs[i].SetActive(i == (int)state);
        }
    }
}
