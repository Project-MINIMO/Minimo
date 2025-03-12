using System;
using System.Linq;
using System.Collections.Generic;
using MinimoShared;
using Cysharp.Threading.Tasks;

using UnityEngine;

public abstract class ProduceObject : BuildingObject
{
    public ProduceData ProduceData { get; private set; }
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
    private TimeManager _timeManager;
    private DateTime _lastUpdateTime;

    public override void Initialize(BuildingData data)
    {
        base.Initialize(data);

        ProduceData = App.GetData<TitleData>().Produce[data.ID];

        _plantHelper = new PlantHelper();
        
        _produceManager = App.GetManager<ProduceManager>();
        _timeManager = App.GetManager<TimeManager>();
        _lastUpdateTime = _timeManager.Time;
    }
    
    public override void Initialize(BuildingDTO buildingDto)
    {
        base.Initialize(buildingDto);

        for (var i = 0; i < buildingDto.ProduceStatus.Length; i++)
        {
            if (buildingDto.ProduceStatus[i] == ProduceSlotStatus.Producing)
            {
                _produceSlots[i] = true;
                
                var produceOption = ProduceData.ProduceOptions[--buildingDto.Recipes[i]];
                var newTask = new ProduceTask(produceOption, i);
                newTask.ReduceRemainTime((int)(_timeManager.Time - buildingDto.ProduceStartAt[i]).TotalSeconds);
                newTask.ChangeState(ActiveState.Instance);
                AllTasks.Add(newTask);

                ActiveTask = newTask;
            }
            else if (buildingDto.ProduceStatus[i] == ProduceSlotStatus.Completed) 
            {
                _produceSlots[i] = true;
                
                var produceOption = ProduceData.ProduceOptions[--buildingDto.Recipes[i]];
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
                
                    var produceOption = ProduceData.ProduceOptions[--buildingDto.Recipes[i]];
                    var newTask = new ProduceTask(produceOption, i);
                    newTask.ChangeState(PendingState.Instance);
                    AllTasks.Add(newTask);
                }
            }
        }
    }

    protected virtual void Update()
    {
        if ((_timeManager.Time - _lastUpdateTime).TotalSeconds < 1f) return;

        _lastUpdateTime = _timeManager.Time;

        if (ActiveTask == null) return;
        
        ActiveTask.Update();
        UpdateRemainTime();
        
        if (ActiveTask is { RemainTime: <= 0 })
        {
            CompleteActiveTask();
        }
    }
    
    protected virtual async UniTask CompleteActiveTask()
    {
        var newCompleteProduce = new BuildingCompleteProduceDTO
        {
            BuildingId = ID,
            SlotIndex = ActiveTask.SlotIndex
        };
        await _buildingManager.CompleteProduce(newCompleteProduce);
        
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

    public void StartPlant(ProduceOption option)
    {
        if (!ProduceData.ProduceOptions.Contains(option)) return;
        if (_isPlanting) return;
        
        var slotIndex = Array.FindIndex(_produceSlots, slot => !slot);
        
        if (slotIndex == -1) return;
        
        var optionIndex = Array.IndexOf(ProduceData.ProduceOptions, option);
        
        _plantHelper.TryPlant(
            option,
            optionIndex,
            slotIndex,
            OnPlant
        );
    }

    protected virtual async UniTask OnPlant(ProduceTask task, int optionIndex)
    {
        _isPlanting = true;
        
        var newStartProduce = new BuildingStartProduceDTO
        {
            BuildingId = ID,
            SlotIndex = task.SlotIndex,
            RecipeId = optionIndex + 1
        };
        
        await _buildingManager.StartProduce(newStartProduce);
        
        _produceSlots[task.SlotIndex] = true;
        
        AllTasks.Add(task);
        Debug.Log($"ProduceTask Added : {task.Data.Results[0].Code}");
        SetNextActiveTask();

        _isPlanting = false;
    }

    public virtual async UniTask StartHarvest()
    {
        if (_isHarvesting) return;
        _isHarvesting = true;
        
        for (var i = AllTasks.Count - 1; i >= 0; i--)
        {
            var task = AllTasks[i];
            if (task.CurrentState is CompletedState)
            {
                var newHarvestProduce = new BuildingHarvestProduceDTO()
                {
                    BuildingId = ID,
                    SlotIndex = task.SlotIndex
                };
                await _buildingManager.HarvestProduce(newHarvestProduce);
                
                task.Harvest();
                AllTasks.RemoveAt(i);
                _produceSlots[task.SlotIndex] = false;
            }
        }

        SetNextActiveTask();
        
        _isHarvesting = false;
    }

    public virtual async UniTask HarvestEarly()
    {
        if (ActiveTask == null) return;
        
        var newInstantProduce = new BuildingInstantProduceDTO
        {
            BuildingId = ID,
            SlotIndex = ActiveTask.SlotIndex
        };
        await _buildingManager.InstantProduceAsync(newInstantProduce);
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