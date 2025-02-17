using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFirstMeteor : QuestBase
{
    public override string ID => "FirstMeteor";
    protected override bool IsClear => CheckClear();
    [SerializeField] private Meteor _meteor;
    
    private GetItemPanel _getItemPanel;
    
    public override void StartQuest()
    {
        base.StartQuest();

        _meteor.enabled = true;
        _getItemPanel = App.GetManager<UIManager>().GetPanel<GetItemPanel>();
    }

    private bool CheckClear()
    {
        if (_getItemPanel == null) 
        {
            return false;
        }
        
        return _getItemPanel.IsComplete;
    }
    
    protected override void ClearQuest()
    {
        base.ClearQuest();
        _meteor.gameObject.SetActive(false);
    }
}
