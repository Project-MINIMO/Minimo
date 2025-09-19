using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.Threading.Tasks;

public abstract class ProduceAdvanced : ProduceObject
{
    public event Action<Minimo> OnMinimoAssigned;

    private MinimoManager _minimoManager;
    public Minimo AssignedMinimo { get; private set; }
    
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
        if (AssignedMinimo != null)
        {
            base.Update();
        }
    }

    public void PlaceMinimo(Minimo minimo)
    {
        if (AssignedMinimo != null)
        {
            UnplaceMinimo();
        }

        AssignedMinimo = minimo;
        
        AssignedMinimo.OnLevelChanged += HandleMinimoLevelChanged;
        AssignedMinimo.AssignTo(this);
        //_minimoManager.OnMinimoAssigned(AssignedMinimo);
        
        ApplyAllAbilities(minimo);
        OnMinimoAssigned?.Invoke(minimo);
    }

    public void UnplaceMinimo()
    {
        if (AssignedMinimo == null) return;
        
        AssignedMinimo.OnLevelChanged -= HandleMinimoLevelChanged;
        //_minimoManager.OnMinimoUnassigned(AssignedMinimo);
        AssignedMinimo.Unassign();
        
        ResetRatios();

        AssignedMinimo = null;
    }
    
    private void HandleMinimoLevelChanged(Minimo minimo, int newLevel)
    {
        ResetRatios();
        ApplyAllAbilities(AssignedMinimo);
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
    
    /*
    public override NotifyType CheckPlantCondition(ProduceData option)
    {
        var result = base.CheckPlantCondition(option);
        if (result == NotifyType.Success)
        {
            if (AssignedMinimo == null)
            {
                return _minimoManager.AssignNearestMinimo(this) 
                    ? NotifyType.Success 
                    : NotifyType.MissMinimo;
            }
        }
        
        return result;
    }
    */

    public override ProduceTask CreateTask(ProduceData option)
    {
        var task = base.CreateTask(option);
        
        task.ApplyTimeReduction(_globalTimeReduction);
        return task;
    }

    public override bool Destroy()
    {
        if (AssignedMinimo != null)
        {
            App.Notification(NotifyType.DeleteBuildingFailMinimo);
            return false;
        }

        return base.Destroy();
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
