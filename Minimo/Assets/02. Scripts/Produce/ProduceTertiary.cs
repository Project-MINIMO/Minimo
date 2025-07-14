public class ProduceTertiary : ProduceAdvanced
{
    public int MaxSlotCount { get; private set; } = 1;

    public void AddSlotCount()
    {
        MaxSlotCount++;
    }

    protected override void CompleteActiveTask()
    {
        var activeTask = ActiveTask;
        
        base.CompleteActiveTask();
        
        AllTasks.Remove(activeTask);
        AllTasks.Add(activeTask);
    }
    
    public override void StartPlant(ProduceData option)
    {
        if (AllTasks.Count >= MaxSlotCount) return;
        
        base.StartPlant(option);
    }

    public void StartHarvest(ProduceTask task)
    {
        if (!AllTasks.Contains(task)) return;
        if (task.CurrentState is not CompletedState) return;
        
        task.ChangeState(EndState.Instance);
        AllTasks.Remove(task);
    }
    
    public override void HarvestEarly()
    {
        var activeTask = ActiveTask;
        
        ActiveTask?.ChangeState(CompletedState.Instance);
        SetNextActiveTask();
        
        AllTasks.Remove(activeTask);
        AllTasks.Add(activeTask);
    }
}
