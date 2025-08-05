using System.Linq;
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

    private void OnEnable()
    {
        if (_produceManager == null) return;
        if (_produceManager.CurrentObject == null) return;
        
        InitHandlers();
    }

    private void InitHandlers()
    {
        var options = _produceManager.CurrentObject.ProduceData;
        var sortingOptions = options.OrderBy(x => x.ResultItems[0].Amount)
            .ThenBy(x => x.ResultItems[0].ID)
            .ToList();
        var i = 0;
        
        for (; i < sortingOptions.Count; i++) 
        {
            var option = sortingOptions[i];
            _plantHandlers[i].SetOption(option);
            
            _plantHandlers[i].gameObject.SetActive(true);
        }

        for (; i < _plantHandlers.Length; i++) 
        {
            _plantHandlers[i].gameObject.SetActive(false);
        }
    }
}
