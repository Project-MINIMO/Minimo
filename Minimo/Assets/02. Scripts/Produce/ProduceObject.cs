using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UniRx;

public enum ProduceState
{
    Idle,
    Produce,
    Complete
}

public abstract class ProduceObject : BuildingObject
{
    public List<ProduceData> ProduceData { get; private set; }
    public List<ProduceTask> AllTasks { get; } = new(); 
    public ProduceTask ActiveTask => AllTasks.FirstOrDefault(t => t.CurrentState is ActiveState);
    
    private ProduceManager _produceManager;
    private PlantHelper _plantHelper;
    
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
            CompleteActiveTask();
        }
    }
    
    protected virtual void CompleteActiveTask()
    {
        ActiveTask.ChangeState(CompletedState.Instance);
        SetNextActiveTask();
    }
    
    protected void SetNextActiveTask()
    {
        if (ActiveTask != null) return;

        AllTasks
            .FirstOrDefault(task => task.CurrentState is PendingState)
            ?.ChangeState(ActiveState.Instance);
    }

    public virtual void StartPlant(ProduceData option)
    {
        if (!ProduceData.Contains(option)) return;

        var optionIndex = ProduceData.IndexOf(option);
        
        _plantHelper.TryPlant(
            option,
            optionIndex,
            OnPlant
        );
    }

    protected virtual void OnPlant(ProduceTask task, int optionIndex)
    {
        task.ApplyTimeRatio(_timeRatio * _globalTimeRatio);
        task.ApplyHarvestRatio(_harvestRatio * _globalHarvestRatio); 
        AllTasks.Add(task);
        Debug.Log($"ProduceTask Added : {task.Data.ResultItems[0].ID}");
        SetNextActiveTask();
    }

    public virtual void StartHarvest()
    {
        for (var i = AllTasks.Count - 1; i >= 0; i--)
        {
            var task = AllTasks[i];
            if (task.CurrentState is not CompletedState) continue;
            
            task.ChangeState(EndState.Instance);
            AllTasks.RemoveAt(i);
        }
    }

    public virtual void HarvestEarly()
    {
        ActiveTask?.ChangeState(CompletedState.Instance);
        SetNextActiveTask();
    }

    public override void OnClickUp()
    {
        base.OnClickUp();

        if (!_editManager.IsEditing.Value)
        {
            _produceManager.Select(this);
        }
    }
    
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