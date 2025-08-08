using UnityEngine;

public class PlantCtrl : MonoBehaviour
{
    [SerializeField] private DragArrowHint _hint;
    
    private PlantHandler[] _plantHandlers;
    private ProduceManager _produceManager;

    private void Awake()
    {
        _plantHandlers = GetComponentsInChildren<PlantHandler>(true);
        foreach (var handler in _plantHandlers)
        {
            handler.OnDragChanged += isDrag =>
            {
                if (_hint != null)
                {
                    _hint.OnDragChanged(isDrag);
                }
            };
        }
        _produceManager = App.GetManager<ProduceManager>();
    }

    private void OnEnable()
    {
        if (_produceManager == null) return;
        if (_produceManager.CurrentObject == null) return;
        
        if (_hint != null)
        {
            _hint.gameObject.SetActive(true);
        }
        
        InitHandlers();
    }

    private void InitHandlers()
    {
        var options = _produceManager.CurrentObject.ProduceData;
        var i = 0;
        
        for (; i < options.Count; i++) 
        {
            _plantHandlers[i].gameObject.SetActive(true);
            
            var option = options[i];
            _plantHandlers[i].SetOption(option);
        }

        for (; i < _plantHandlers.Length; i++) 
        {
            _plantHandlers[i].gameObject.SetActive(false);
        }
    }
}
