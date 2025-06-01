using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceSecondary : ProduceAdvanced
{
    private PrimaryPanel _primaryPanel;
    
    public override void Initialize(int id)
    {
        base.Initialize(id);

        _primaryPanel = App.GetManager<UIManager>().GetPanel<PrimaryPanel>();
    }
    
    public override void OpenUI()
    {
        switch (ActiveTask.CurrentState)
        {
            case ActiveState:
                _primaryPanel.OpenPanel(ProduceState.Produce);
                break;
            
            default:
                _primaryPanel.OpenPanel(ProduceState.Idle);
                break;
        }
    }
    
    public override void CloseUI()
    {
        _primaryPanel.ClosePanel();
    }
}
