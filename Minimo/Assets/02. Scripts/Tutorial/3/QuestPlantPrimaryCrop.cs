using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestPlantPrimaryCrop : QuestBase
{
    public override string ID => "PlantPrimary_Crop";
    protected override bool IsClear => CheckClear();

    [SerializeField] private Transform _builidngParent;
    private ProduceObject _produceObject;
    private List<StorageBtn> _items = new List<StorageBtn>();
    
    public override void StartQuest()
    {
        base.StartQuest();

        for (var i = 0; i < _builidngParent.childCount; i++) 
        {
            Debug.Log(_builidngParent.GetChild(i).gameObject.name);
            if (string.Equals(_builidngParent.GetChild(i).gameObject.name, "Building_Farm(Clone)"))
            {
                _produceObject = _builidngParent.GetChild(i).GetComponent<ProduceObject>();
                break;
            }
        }
        
        var storagePanel = App.GetManager<UIManager>().GetPanel<StoragePanel>();
        _items.Add(storagePanel.GetStorageBtn("Item_Wheat"));
        _items.Add(storagePanel.GetStorageBtn("Item_Corn"));
        _items.Add(storagePanel.GetStorageBtn("Item_Pumpkin"));
        _items.Add(storagePanel.GetStorageBtn("Item_Sugarcane"));
        _items.Add(storagePanel.GetStorageBtn("Item_Pepper"));
    }
    
    private bool CheckClear()
    {
        if (_items.Count > 0 && _items.Any(x => x.CanShow))
        {
            return true;
        }
        
        if (_produceObject == null) 
        {
            return false;
        }
        return _produceObject.AllTasks.Count > 0;
    }
}