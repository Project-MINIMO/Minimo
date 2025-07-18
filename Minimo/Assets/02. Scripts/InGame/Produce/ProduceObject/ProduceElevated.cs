using System;

public class ProduceElevated : ProduceAdvanced
{
    public event Action<int> OnTaskCountChanged;
    public event Action OnMaxSlotCountChanged;
    
    public void AddSlotCount()
    {
        MaxSlotCount++;
        OnMaxSlotCountChanged?.Invoke();
    }

    public override ProduceTask CreateTask(ProduceData option)
    {
        var task = base.CreateTask(option);
        
        OnTaskCountChanged?.Invoke(AllTasks.Count);
        return task;
    }
    
    internal override void StartHarvest()
    {
        var before = AllTasks.Count;
        
        base.StartHarvest();

        if (AllTasks.Count != before)
        {
            OnTaskCountChanged?.Invoke(AllTasks.Count);
        }
    }
}
