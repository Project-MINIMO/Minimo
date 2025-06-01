using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceTertiary : ProduceAdvanced
{
    private AdvancedPanel _advancedPanel;
    
    public override void Initialize(BuildingData data)
    {
        base.Initialize(data);

        _advancedPanel = App.GetManager<UIManager>().GetPanel<AdvancedPanel>();
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
