using UnityEngine;
using UniRx;

public abstract class ProduceAdvanced : ProduceObject
{
    public Transform MinimoWorkingPosition;
    public bool IsMinimoWorking => MinimoWorkingPosition != null && MinimoWorkingPosition.childCount > 0;
    public string AnimTrigger;

    private Minimo _placedMinimo;
    
    private float _globalTimeReduction;
    
    protected override float TimeRatio => _timeRatio * _globalTimeRatio;
    private float _timeRatio = 1f;
    
    protected override float HarvestRatio => _harvestRatio * _globalHarvestRatio;
    private float _harvestRatio = 1f;
    
    protected override float ExpRatio => _expRatio * _globalExpRatio;
    private float _expRatio = 1f;
    
    protected override void Awake()
    {
        base.Awake();

        MinimoWorkingPosition = transform.GetChild(2);

        var minimoManager = App.GetManager<MinimoManager>();
        minimoManager
            .GlobalTimeReduction
            .Subscribe(value =>
            {
                _globalTimeReduction = value;
                
                foreach (var task in AllTasks)
                {
                    task.ApplyTimeReduction(value); 
                }
            })
            .AddTo(this);
        _globalTimeReduction = minimoManager.GlobalTimeReduction.Value;
    }

    public void PlaceMinimo(Minimo minimo)
    {
        _placedMinimo?.SetChillState();
        _placedMinimo = minimo;
    }
    
    public override NotifyType CheckPlantCondition(ProduceData option)
    {
        var result = base.CheckPlantCondition(option);
        if (result == NotifyType.Success)
        {
            return _placedMinimo == null
                ? NotifyType.MissMinimo
                : NotifyType.Success;
        }
        
        return result;
    }

    public override ProduceTask CreateTask(ProduceData option)
    {
        var task = base.CreateTask(option);
        
        task.ApplyTimeReduction(_globalTimeReduction);
        return task;
    }
    
    #region Apply Minimo Abilities
    public void ApplyTimeRatio(float value)
    {  
        _timeRatio = 1 - value / 100;
        
        foreach (var task in AllTasks)
        {
            task.ApplyTimeRatio(TimeRatio);
        }
    }
    
    public void ApplyHarvestRatio(float value)
    {  
        _harvestRatio = 1 - value / 100;
        
        foreach (var task in AllTasks)
        {
            task.ApplyHarvestRatio(HarvestRatio);
        }
    }
    
    public void ApplyExpRatio(float value)
    {
        _expRatio = 1 + value / 100;
        
        foreach (var task in AllTasks)
        {
            task.ApplyExpRatio(ExpRatio);
        }
    }
    #endregion
}
