public class ProduceSecondary : ProduceAdvanced
{
    internal override void StartPlant(ProduceData option)
    {
        if (AllTasks.Count > 0)
        {
            return;
        }

        base.StartPlant(option);
    }
    
    public override void OnClickUp()
    {
        if (AllTasks.Count > 0 && AllTasks[0].CurrentState is CompletedState)
        {
            StartHarvest();
        }
        
        base.OnClickUp();
    }
}
