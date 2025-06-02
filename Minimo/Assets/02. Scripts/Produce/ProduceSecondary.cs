using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceSecondary : ProduceAdvanced
{
    private PrimaryPanel _primaryPanel;
    
    protected override void Awake()
    {
        base.Awake();

        _primaryPanel = App.GetManager<UIManager>().GetPanel<PrimaryPanel>();
    }
    
    protected override void OnPlant(ProduceTask task, int optionIndex)
    {
        if (AllTasks.Count > 0)
        {
            return;
        }
        
        base.OnPlant(task, optionIndex);
    }
    
    public override void OnClickUp()
    {
        base.OnClickUp();

        if (AllTasks.Count > 0 && AllTasks[0].CurrentState is CompletedState)
        {
            StartHarvest();
        }
    }
    
    public override void OpenUI()
    {
        if (ActiveTask == null)
        {
            if (AllTasks.Count == 0)
            {
                _primaryPanel.OpenPanel(ProduceState.Idle);
            }
            else
            {
                _primaryPanel.ClosePanel();
            }
        }
        else
        {
            _primaryPanel.OpenPanel(ProduceState.Produce);
        }
    }
    
    public override void CloseUI()
    {
        _primaryPanel.ClosePanel();
    }
}
