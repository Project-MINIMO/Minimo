public class ProduceTertiary : ProduceAdvanced
{
    public int MaxSlotCount { get; private set; } = 1;

    public void AddSlotCount()
    {
        MaxSlotCount++;
    }

    internal override void StartPlant(ProduceData option)
    {
        if (AllTasks.Count >= MaxSlotCount) return;
        
        base.StartPlant(option);
    }
}
