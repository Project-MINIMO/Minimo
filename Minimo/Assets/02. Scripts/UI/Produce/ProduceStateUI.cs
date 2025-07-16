using UnityEngine;
using UniRx;

public class ProduceStateUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _stateUIs;
    
    private void Start()
    {
        App.GetManager<EditManager>().IsEditing
            .Subscribe((isEditing) =>
            {
                gameObject.SetActive(!isEditing);
            }).AddTo(gameObject);
        
        var obj = GetComponentInParent<ProduceObject>();
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
