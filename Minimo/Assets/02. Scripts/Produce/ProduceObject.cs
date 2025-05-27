using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public abstract class ProduceObject : BuildingObject
{
    public List<ProduceData> ProduceData { get; private set; }
    public List<ProduceTask> AllTasks { get; } = new(); 
    public ProduceTask ActiveTask { get; private set; }
    public virtual bool IsPrimary => false;
    
    public Transform MinimoWorkingPosition;
    public bool IsMinimoWorking => MinimoWorkingPosition != null && MinimoWorkingPosition.childCount > 0;
    public string AnimTrigger;
    
    private readonly bool[] _produceSlots = new bool[5];
    private bool _isPlanting = false;
    private bool _isHarvesting = false;

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
    
    public override void Initialize(int id)
    {
        base.Initialize(id);

        /*
        for (var i = 0; i < buildingDto.ProduceStatus.Length; i++)
        {
            if (buildingDto.ProduceStatus[i] == ProduceSlotStatus.Producing)
            {
                _produceSlots[i] = true;
                
                var produceOption = new ProduceData();//ProduceData.ProduceOptions[--buildingDto.Recipes[i]];
                var newTask = new ProduceTask(produceOption, i);
                newTask.ReduceRemainTime((int)(_timeManager.Time - buildingDto.ProduceStartAt[i]).TotalSeconds);
                newTask.ChangeState(ActiveState.Instance);
                AllTasks.Add(newTask);

                ActiveTask = newTask;
            }
            else if (buildingDto.ProduceStatus[i] == ProduceSlotStatus.Completed) 
            {
                _produceSlots[i] = true;
                
                var produceOption = new ProduceData();//ProduceData.ProduceOptions[--buildingDto.Recipes[i]];
                var newTask = new ProduceTask(produceOption, i);
                newTask.ChangeState(CompletedState.Instance);
                newTask.ReduceRemainTime(produceOption.Time);
                AllTasks.Add(newTask);
            }
            else
            {
                if (buildingDto.Recipes[i] == 0)
                {
                    _produceSlots[i] = false;
                }
                else
                {
                    _produceSlots[i] = true;

                    var produceOption = new ProduceData();//ProduceData.ProduceOptions[--buildingDto.Recipes[i]];
                    var newTask = new ProduceTask(produceOption, i);
                    newTask.ChangeState(PendingState.Instance);
                    AllTasks.Add(newTask);
                }
            }
        }
        */
    }

    protected virtual void Update()
    {
        if (Time.time - _lastUpdateTime < 1f) return;

        _lastUpdateTime = Time.time;

        if (ActiveTask == null) return;
        
        ActiveTask.Update();
        UpdateRemainTime();
        
        if (ActiveTask is { RemainTime: <= 0 })
        {
            CompleteActiveTask();
        }
    }
    
    protected virtual void CompleteActiveTask()
    {
        ActiveTask.ChangeState(CompletedState.Instance);
        ActiveTask = null;
        SetNextActiveTask();
    }
    
    private void SetNextActiveTask()
    {
        if (ActiveTask != null)
        {
            return;
        }
        
        ActiveTask = AllTasks.FirstOrDefault(task => task.CurrentState is PendingState);

        if (ActiveTask != null)
        {
            ActiveTask.ChangeState(ActiveState.Instance);
            UpdateRemainTime();
        }
    }
    
    private void UpdateRemainTime()
    {
        if (_produceManager.CurrentProduceObject == this && ActiveTask != null)
        {
            _produceManager.SetRemainTime(ActiveTask.RemainTime);
        }
    }

    public void StartPlant(ProduceData option)
    {
        //if (!ProduceData.ProduceOptions.Contains(option)) return;
        if (_isPlanting) return;
        
        var slotIndex = Array.FindIndex(_produceSlots, slot => !slot);
        
        if (slotIndex == -1) return;

        var optionIndex = 0;//Array.IndexOf(ProduceData.ProduceOptions, option);
        
        _plantHelper.TryPlant(
            option,
            optionIndex,
            slotIndex,
            OnPlant
        );
    }

    protected virtual void OnPlant(ProduceTask task, int optionIndex)
    {
        _isPlanting = true;
        
        _produceSlots[task.SlotIndex] = true;
        
        AllTasks.Add(task);
        Debug.Log($"ProduceTask Added : {task.Data.ResultItems[0].ID}");
        SetNextActiveTask();

        _isPlanting = false;
    }

    public virtual void StartHarvest()
    {
        if (_isHarvesting) return;
        _isHarvesting = true;
        
        for (var i = AllTasks.Count - 1; i >= 0; i--)
        {
            var task = AllTasks[i];
            if (task.CurrentState is CompletedState)
            {
                task.Harvest();
                AllTasks.RemoveAt(i);
                _produceSlots[task.SlotIndex] = false;
            }
        }

        SetNextActiveTask();
        
        _isHarvesting = false;
    }

    public virtual void HarvestEarly()
    {
        if (ActiveTask == null) return;
        
        ActiveTask.Harvest();
        ActiveTask = null;

        SetNextActiveTask();
    }
    
    public void OrganizeTasks()
    {
        AllTasks.RemoveAll(task => task.CurrentState is EndState);
    }

    public override void OnClickUp()
    {
        base.OnClickUp();

        if (!_editManager.IsEditing.Value)
        {
            _produceManager.ActiveProduce(this);

            if (ActiveTask != null)
            {
                _produceManager.SetRemainTime(ActiveTask.RemainTime);
            }
        }
    }
}