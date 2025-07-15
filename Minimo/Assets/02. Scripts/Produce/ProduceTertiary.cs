using System;

public class ProduceTertiary : ProduceAdvanced
{
    public int MaxSlotCount { get; private set; } = 1;
    
    public event Action<int> OnTaskCountChanged;

    public void AddSlotCount()
    {
        MaxSlotCount++;
    }

    internal override void StartPlant(ProduceData option)
    {
        if (AllTasks.Count >= MaxSlotCount) return;
        
        base.StartPlant(option);
    }

    internal override void OnPlant(ProduceTask task)
    {
        base.OnPlant(task);
        
        OnTaskCountChanged?.Invoke(AllTasks.Count);
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
