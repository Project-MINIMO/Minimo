using UnityEngine;

public class PlantCtrl : MonoBehaviour
{
    private PlantOptionSlot[] _plantOptionSlots;
    private ProduceManager _produceManager;

    private void Awake()
    {
        _plantOptionSlots = GetComponentsInChildren<PlantOptionSlot>(true);
        _produceManager = App.GetManager<ProduceManager>();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);

        if (isActive)
        {
            InitOptionButtons();
        }
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
