using UniRx;
using UnityEngine;

public class ProduceManager : ManagerBase
{
    public ProduceObject CurrentProduceObject { get; private set; }
    
    public ReactiveProperty<int> CurrentRemainTime { get; } = new(-1);

    private float _lastUpdateTime;
    
    private void Update()
    {
        if (Time.time - _lastUpdateTime < 1f) return;

        _lastUpdateTime = Time.time;

        SetRemainTime();
    }
    
    public void ActiveProduce(ProduceObject produceObject)
    {
        if (CurrentProduceObject && CurrentProduceObject != produceObject)
        {
            CurrentProduceObject.CloseUI();
        }
        
        CurrentProduceObject = produceObject;
        CurrentProduceObject.OpenUI();
        
        SetRemainTime();
    }
    
    public void DeactiveProduce()
    {
        CurrentProduceObject.CloseUI();
        CurrentProduceObject = null;
        CurrentRemainTime.Value = -1;
    }

    private void SetRemainTime()
    {
        if (CurrentProduceObject == null) return;
        if (CurrentProduceObject.ActiveTask == null)
        {
            CurrentRemainTime.Value = -1;
            return;
        }
        
        CurrentRemainTime.Value = CurrentProduceObject.ActiveTask.RemainTime;
    }

    public void HarvestEarly()
    {
        if (!CurrentProduceObject) return;
        
        CurrentProduceObject.HarvestEarly();
        CurrentRemainTime.Value = -1;

        SetRemainTime();
    }
}
