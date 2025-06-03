using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : ManagerBase
{
    public List<QuestData> ActiveQuests { get; private set; } = new();

    private QuestPanel _questPanel;

    protected override void Awake()
    {
        base.Awake();

        _questPanel = App.GetManager<UIManager>().GetPanel<QuestPanel>();
    }
    
    public void AddQuest(QuestData quest)
    {
        ActiveQuests.Add(quest);
        _questPanel.UpdateQuest(quest.Type);
    }

    public void RemoveQuest(QuestData quest)
    {
        ActiveQuests.Remove(quest);
        _questPanel.UpdateQuest(0);
    }
}
