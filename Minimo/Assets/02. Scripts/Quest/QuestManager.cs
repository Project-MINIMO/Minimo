using System.Collections.Generic;

public class QuestManager : ManagerBase
{
    public List<DetailQuestData> ActiveQuests { get; private set; } = new();

    private QuestSummaryPanel _questSummaryPanel;
    private QuestListPanel _questListPanel;

    protected override void Awake()
    {
        base.Awake();

        _questSummaryPanel = App.GetManager<UIManager>().GetPanel<QuestSummaryPanel>();
        _questListPanel = App.GetManager<UIManager>().GetPanel<QuestListPanel>();
    }
    
    public void AddQuest(DetailQuestData quest)
    {
        ActiveQuests.Add(quest);
        
        _questSummaryPanel.UpdateQuest();
        _questListPanel.UpdateQuest(quest.Type);
    }

    public void RemoveQuest(DetailQuestData quest)
    {
        ActiveQuests.Remove(quest);
        
        _questSummaryPanel.UpdateQuest();
        _questListPanel.UpdateQuest(quest.Type);
    }
}
