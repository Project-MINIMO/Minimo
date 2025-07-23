using UnityEngine;
using UniRx;
using System;

public abstract class ProduceAdvanced : ProduceObject
{
    public event Action<Minimo> OnMinimoAssigned;
    private MinimoManager _minimoManager;
    private Minimo _placedMinimo;
    
    public Transform MinimoWorkingPosition;
    public string AnimTrigger;
    
    protected override float TimeRatio => _timeRatio * _globalTimeRatio;
    protected override float HarvestRatio => _harvestRatio * _globalHarvestRatio;
    protected override float ExpRatio => _expRatio * _globalExpRatio;
    
    private float _timeRatio = 1f;
    private float _harvestRatio = 1f;
    private float _expRatio = 1f;
    private float _globalTimeReduction;
    
    protected override void Awake()
    {
        base.Awake();

        MinimoWorkingPosition = transform.GetChild(2);

        _minimoManager = App.GetManager<MinimoManager>();
        _minimoManager.GlobalTimeReduction
            .Subscribe(value =>
            {
                _globalTimeReduction = value;
                
                foreach (var task in AllTasks)
                {
                    task.ApplyTimeReduction(value); 
                }
            })
            .AddTo(this);
        _globalTimeReduction = _minimoManager.GlobalTimeReduction.Value;
    }
    
    protected override void Update()
    {
        if (_placedMinimo != null)
        {
            base.Update();
        }
    }

    public void PlaceMinimo(Minimo minimo)
    {
        if (_placedMinimo != null)
        {
            UnplaceMinimo();
        }

        _placedMinimo = minimo;
        
        _placedMinimo.OnLevelChanged += HandleMinimoLevelChanged;
        _placedMinimo.AssignTo(this);
        _minimoManager.OnMinimoAssigned(_placedMinimo);
        
        ApplyAllAbilities(minimo);
        OnMinimoAssigned?.Invoke(minimo);
    }

    public void UnplaceMinimo()
    {
        if (_placedMinimo == null) return;
        
        _placedMinimo.OnLevelChanged -= HandleMinimoLevelChanged;
        _minimoManager.OnMinimoUnassigned(_placedMinimo);
        _placedMinimo.Unassign();
        
        ResetRatios();

        _placedMinimo = null;
    }
    
    private void HandleMinimoLevelChanged(Minimo minimo, int newLevel)
    {
        ResetRatios();
        ApplyAllAbilities(_placedMinimo);
    }

    private void ApplyAllAbilities(Minimo minimo)
    {
        foreach (var ability in minimo.Abilities)
        {
            ability.Apply(this);
        }
    }
    
    private void ResetRatios()
    {
        ApplyTimeRatio(0);
        ApplyHarvestRatio(0);
        ApplyExpRatio(0);
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
