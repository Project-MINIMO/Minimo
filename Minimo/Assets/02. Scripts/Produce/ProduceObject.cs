using System.Linq;
using System.Collections.Generic;

using UnityEngine;

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
    
    private PlantHelper _plantHelper;
    
    private ProduceManager _produceManager;
    private float _lastUpdateTime;

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
        if (Time.time - _lastUpdateTime < 1f) return;

        _lastUpdateTime = Time.time;

        if (ActiveTask == null) return;
        
        ActiveTask.Update();
        
        if (ActiveTask is { RemainTime: <= 0 })
        {
            CompleteActiveTask();
        }
    }
    
    protected virtual void CompleteActiveTask()
    {
        ActiveTask.ChangeState(CompletedState.Instance);
        SetNextActiveTask();
    }
    
    private void SetNextActiveTask()
    {
        if (ActiveTask != null)
        {
            return;
        }
        
        var pendingTask = AllTasks.FirstOrDefault(task => task.CurrentState is PendingState);
        pendingTask?.ChangeState(ActiveState.Instance);
    }

    public void StartPlant(ProduceData option)
    {
        if (!ProduceData.Contains(option)) return;

        var optionIndex = 0;//Array.IndexOf(ProduceData.ProduceOptions, option);
        
        _plantHelper.TryPlant(
            option,
            optionIndex,
            OnPlant
        );
    }

    protected virtual void OnPlant(ProduceTask task, int optionIndex)
    {
        AllTasks.Add(task);
        Debug.Log($"ProduceTask Added : {task.Data.ResultItems[0].ID}");
        SetNextActiveTask();
    }

    public virtual void StartHarvest()
    {
        for (var i = AllTasks.Count - 1; i >= 0; i--)
        {
            var task = AllTasks[i];
            if (task.CurrentState is CompletedState)
            {
                task.Harvest();
                AllTasks.RemoveAt(i);
            }
        }
    }

    public virtual void HarvestEarly()
    {
        ActiveTask?.Harvest();
    }

    public override void OnClickUp()
    {
        base.OnClickUp();

        if (!_editManager.IsEditing.Value)
        {
            _produceManager.ActiveProduce(this);
        }
    }

    public abstract void OpenUI();
    public abstract void CloseUI();
}