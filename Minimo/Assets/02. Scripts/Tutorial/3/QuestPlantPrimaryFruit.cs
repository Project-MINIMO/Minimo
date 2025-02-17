using System.Collections.Generic;
using UnityEngine;

public class QuestPlantPrimaryFruit : QuestBase
{
    public override string ID => "PlantPrimary_Fruit";
    protected override bool IsClear => CheckClear();

    [SerializeField] private Transform _builidngParent;
    private ProduceObject _produceObject;
    private StorageBtn _storageBtn;
    
    public override void StartQuest()
    {
        base.StartQuest();

        for (var i = 0; i < _builidngParent.childCount; i++) 
        {
            if (string.Equals(_builidngParent.GetChild(i).gameObject.name, "Building_Orchard(Clone)"))
            {
                _produceObject = _builidngParent.GetChild(i).GetComponent<ProduceObject>();
                break;
            }
        }

        _storageBtn = App.GetManager<UIManager>().GetPanel<StoragePanel>().GetStorageBtn("Item_OrchardWood");
    }

    private bool CheckClear()
    {
        if (_storageBtn != null && _storageBtn.CanShow)
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