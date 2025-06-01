using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlantCtrl : UIBase
{
    private PlantOptionSlot[] _plantOptionSlots;
    private ProduceManager _produceManager;

    public override void Initialize()
    {
        _plantOptionSlots = GetComponentsInChildren<PlantOptionSlot>(true);
        _produceManager = App.GetManager<ProduceManager>();
    }
   
    public override void OpenPanel()
    {
        base.OpenPanel();
        
        InitOptionButtons();
    }

    private void InitOptionButtons()
    {
        var options = _produceManager.CurrentProduceObject.ProduceData;

        var i = 0;
        
        for (; i < options.Count; i++) 
        {
            var option = options[i];
            _plantOptionSlots[i].gameObject.SetActive(true);
            _plantOptionSlots[i].SetOption(option);
        }

        for (; i < _plantOptionSlots.Length; i++) 
        {
            _plantOptionSlots[i].gameObject.SetActive(false);
        }
    }
}
