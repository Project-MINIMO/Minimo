using System;

public class ProduceTertiary : ProduceAdvanced
{
    public event Action<int> OnTaskCountChanged;

    public void AddSlotCount()
    {
        MaxSlotCount++;
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
