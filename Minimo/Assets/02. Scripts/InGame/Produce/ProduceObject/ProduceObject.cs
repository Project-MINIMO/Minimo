using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

public abstract class ProduceObject : BuildingObject
{
    public List<ProduceData> ProduceData { get; private set; }
    public List<ProduceTask> AllTasks { get; } = new(); 
    public ProduceTask ActiveTask => AllTasks.FirstOrDefault(t => t.CurrentState is ActiveState);
    public ProduceState CurrentState => GetCurrentProduceState();
    public event Action<ProduceState> OnProduceStateChanged;
    public int MaxSlotCount { get; protected set; } = 1;
    
    protected ProduceManager _produceManager;
    private PlantEffectCtrl _plantEffect;
    
    private float _lastUpdateTime;

    protected virtual float TimeRatio => _globalTimeRatio;
    protected float _globalTimeRatio;
    
    protected virtual float HarvestRatio => _globalHarvestRatio;
    protected float _globalHarvestRatio;
    
    protected virtual float ExpRatio => _globalExpRatio;
    protected float _globalExpRatio;

    protected override void Awake()
    {
        base.Awake();
        
        var minimoManager = App.GetManager<MinimoManager>();
        
        minimoManager
            .GlobalTimeRatio
            .Subscribe(value =>
            {
                _globalTimeRatio = value;
                foreach (var task in AllTasks)
                {
                    task.ApplyTimeRatio(TimeRatio); 
                }
            })
            .AddTo(this);
        _globalTimeRatio = minimoManager.GlobalTimeRatio.Value;
        
        minimoManager
            .GlobalHarvestRatio
            .Subscribe(value =>
            {
                _globalHarvestRatio = value;
                
                foreach (var task in AllTasks)
                {
                    task.ApplyHarvestRatio(HarvestRatio); 
                }
            })
            .AddTo(this);
        _globalHarvestRatio = minimoManager.GlobalHarvestRatio.Value;
        
        minimoManager
            .GlobalExpRatio
            .Subscribe(value =>
            {
                _globalExpRatio = value;
                
                foreach (var task in AllTasks)
                {
                    task.ApplyHarvestRatio(ExpRatio); 
                }
            })
            .AddTo(this);
        _globalExpRatio = minimoManager.GlobalExpRatio.Value;
    }
    
    public override async Task Initialize(Building data)
    {
        await base.Initialize(data);

        ProduceData = App.GetData<TitleData>().GroupedProduce[data.Code];
        
        _plantEffect = GetComponentInChildren<PlantEffectCtrl>();
        
        _produceManager = App.GetManager<ProduceManager>();
        _lastUpdateTime = Time.time;
    }

    protected virtual void Update()
    {
        if (Time.time - _lastUpdateTime < 0.1f) return;
        
        _lastUpdateTime = Time.time;

        if (ActiveTask == null) return;

        var task = ActiveTask;
        task.Update();
        
        if (task.CurrentState is CompletedState)
        {
            SetNextActiveTask();
        }
    }
    
    private void SetNextActiveTask()
    {
        if (ActiveTask != null) return;

        AllTasks
            .FirstOrDefault(task => task.CurrentState is PendingState)
            ?.ChangeState(ActiveState.Instance);

        GetCurrentProduceState();
    }
    
    protected ProduceState GetCurrentProduceState()
    {
        if (AllTasks.Any(x => x.CurrentState is CompletedState))
        {
            OnProduceStateChanged?.Invoke(ProduceState.Complete);
            return ProduceState.Complete;
        }

        var state = ActiveTask != null ? ProduceState.Produce : ProduceState.Idle;
        OnProduceStateChanged?.Invoke(state);
        return state;
    }
    
    public override void OnClickUp()
    {
        base.OnClickUp();

        if (EditManager.IsTileEditing.Value) return;
        
        if (!EditManager.IsBuildingEditing.Value)
        {
            _produceManager.Select(this);
        }
    }

    public virtual NotifyType CheckPlantCondition(ProduceData option)
    {
        if (AllTasks.Count >= MaxSlotCount) return NotifyType.SlotLack;
        if (!ProduceData.Contains(option)) return NotifyType.InvalidOption;

        return NotifyType.Success;
    }
    
    public virtual ProduceTask CreateTask(ProduceData option)
    {
        var task = new ProduceTask(option);
        task.ApplyTimeRatio(TimeRatio);
        task.ApplyHarvestRatio(HarvestRatio); 
        task.ApplyExpRatio(ExpRatio);
        AllTasks.Add(task);
        _plantEffect.PlayEffect(task.Data.MaterialItems.Select(x => x.ID).ToArray());
        
        SetNextActiveTask();
        return task;
    }
    
    #region Produce
    internal virtual void StartHarvest()
    {
        for (var i = AllTasks.Count - 1; i >= 0; i--)
        {
            var task = AllTasks[i];
            if (task.CurrentState is not CompletedState) continue;
            if (!AccountInfo.Instance.CanKeepItem(task.Data.ResultItems[0].ID)) return;
            
            task.ChangeState(EndState.Instance);
            AllTasks.RemoveAt(i);
        }

        GetCurrentProduceState();
    }

    internal void Skip()
    {
        ActiveTask?.ChangeState(CompletedState.Instance);
        SetNextActiveTask();
    }
    #endregion
}