using System;
using System.Linq;
using System.Collections.Generic;

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
    
    private ProduceManager _produceManager;
    private PlantHelper _plantHelper;
    private PlantEffectCtrl _plantEffect;
    
    private float _lastUpdateTime;
    
    private float _timeRatio = 1f;
    private float _globalTimeRatio;

    private float _harvestRatio = 1f;
    private float _globalHarvestRatio;

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
                    task.ApplyTimeRatio(_timeRatio * _globalTimeRatio); 
                }
            })
            .AddTo(this);
        
        minimoManager
            .GlobalHarvestRatio
            .Subscribe(value =>
            {
                _globalHarvestRatio = value;
                
                foreach (var task in AllTasks)
                {
                    task.ApplyHarvestRatio(_harvestRatio * _globalHarvestRatio); 
                }
            })
            .AddTo(this);
    }
    
    public override void Initialize(BuildingData data)
    {
        base.Initialize(data);

        ProduceData = App.GetData<TitleData>().GroupedProduce[data.Name];

        _plantHelper = new PlantHelper();
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
    
    private ProduceState GetCurrentProduceState()
    {
        if (AllTasks.Any(x => x.CurrentState is CompletedState))
        {
            OnProduceStateChanged?.Invoke(ProduceState.Complete);
            return ProduceState.Complete;
        }

        if (ActiveTask != null)
        {
            OnProduceStateChanged?.Invoke(ProduceState.Produce);
            return ProduceState.Produce;
        }
        else
        {
            OnProduceStateChanged?.Invoke(ProduceState.Idle);
            return ProduceState.Idle;
        }
    }
    
    public override void OnClickUp()
    {
        base.OnClickUp();

        if (!_editManager.IsEditing.Value)
        {
            _produceManager.Select(this);
        }
    }
    
    #region Produce
    internal virtual void StartPlant(ProduceData option)
    {
        if (AllTasks.Count >= MaxSlotCount) return;
        if (!ProduceData.Contains(option)) return;

        _plantHelper.TryPlant(option, OnPlant);
    }

    internal virtual void OnPlant(ProduceTask task)
    {
        task.ApplyTimeRatio(_timeRatio * _globalTimeRatio);
        task.ApplyHarvestRatio(_harvestRatio * _globalHarvestRatio); 
        AllTasks.Add(task);
        
        _plantEffect.PlayEffect(task.Materials.Select(x => x.ID).ToArray());
        
        SetNextActiveTask();
    }
    
    internal virtual void StartHarvest()
    {
        for (var i = AllTasks.Count - 1; i >= 0; i--)
        {
            var task = AllTasks[i];
            if (task.CurrentState is not CompletedState) continue;
            
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
    
    #region Apply Minimo Abilities
    public void ApplyTimeRatio(float value)
    {  
        _timeRatio = 1 - value / 100;
        
        foreach (var task in AllTasks)
        {
            task.ApplyTimeRatio(_timeRatio * _globalTimeRatio);
        }
    }
    
    public void ApplyHarvestRatio(float value)
    {  
        _harvestRatio = 1 - value / 100;
        
        foreach (var task in AllTasks)
        {
            task.ApplyHarvestRatio(_harvestRatio * _globalHarvestRatio);
        }
    }
    #endregion
}