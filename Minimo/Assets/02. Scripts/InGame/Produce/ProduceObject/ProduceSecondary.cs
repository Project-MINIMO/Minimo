public class ProduceSecondary : ProduceAdvanced
{
    public override void OnClickUp()
    {
        if (AllTasks.Count > 0 && AllTasks[0].CurrentState is CompletedState)
        {
            StartHarvest();
        }
        
        base.OnClickUp();
    }
}
