public class ProduceTertiary : ProduceAdvanced
{
    public int MaxSlotCount { get; private set; } = 1;
    
    private AdvancedPanel _advancedPanel;
    private WishPanel _wishPanel;
    
    public override void Initialize(BuildingData data)
    {
        base.Initialize(data);

        _advancedPanel = App.GetManager<UIManager>().GetPanel<AdvancedPanel>();
        _wishPanel = App.GetManager<UIManager>().GetPanel<WishPanel>();
    }

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

    public void StartHarvest(ProduceTask task)
    {
        if (!AllTasks.Contains(task)) return;
        if (task.CurrentState is not CompletedState) return;
        
        task.Harvest();
        AllTasks.Remove(task);
    }
    
    public override void OpenUI()
    {
        if (BuildingData.ID == 23)
        {
            _wishPanel.OpenPanel();
        }
        else
        {
            _advancedPanel.OpenPanel();
        }
    }
    
    public override void CloseUI()
    {
        if (BuildingData.ID == 23)
        {
            _wishPanel.ClosePanel();
        }
        else
        {
            _advancedPanel.ClosePanel();
        }
    }
}
