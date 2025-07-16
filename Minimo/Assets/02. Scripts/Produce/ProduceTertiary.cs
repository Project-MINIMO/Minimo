using System;

public class ProduceTertiary : ProduceAdvanced
{
    public event Action<int> OnTaskCountChanged;
    public event Action OnMaxSlotCountChanged;

    public void AddSlotCount()
    {
        MaxSlotCount++;
        OnMaxSlotCountChanged?.Invoke();
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
