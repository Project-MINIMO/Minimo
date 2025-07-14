using UnityEngine;

public class PlantCtrl : MonoBehaviour
{
    private PlantHandler[] _plantHandlers;
    private ProduceManager _produceManager;

    private void Awake()
    {
        _plantHandlers = GetComponentsInChildren<PlantHandler>(true);
        _produceManager = App.GetManager<ProduceManager>();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);

        if (isActive)
        {
            InitHandlers();
        }
    }

    private void InitHandlers()
    {
        var options = _produceManager.CurrentObject.ProduceData;

        var i = 0;
        
        for (; i < options.Count; i++) 
        {
            var option = options[i];
            _plantHandlers[i].SetOption(option);
            
            _plantHandlers[i].gameObject.SetActive(true);
        }

        for (; i < _plantHandlers.Length; i++) 
        {
            _plantHandlers[i].gameObject.SetActive(false);
        }
    }
}
