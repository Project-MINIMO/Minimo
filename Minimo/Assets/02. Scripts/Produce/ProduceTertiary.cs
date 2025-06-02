using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceTertiary : ProduceAdvanced
{
    public int MaxSlotCount { get; private set; } = 1;
    
    private AdvancedPanel _advancedPanel;
    
    public override void Initialize(BuildingData data)
    {
        base.Initialize(data);

        _advancedPanel = App.GetManager<UIManager>().GetPanel<AdvancedPanel>();
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
        _advancedPanel.OpenPanel();
    }
    
    public override void CloseUI()
    {
        _advancedPanel.ClosePanel();
    }
}
