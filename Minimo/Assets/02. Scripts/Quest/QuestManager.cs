using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public enum QuestCondition
{
    Normal,
    Choice,
    Quiz
}

public enum ClearType
{
    UserLevel,
    Plant,
    Harvest,
    Craft,
    Wish,
    Build
}

public enum RewardType
{
    Gold,
    Exp,
    Item
}

public enum QuestType
{
    Guide,
    Story,
    Constellation,
    Side,
    Wish
}

public class QuestManager : ManagerBase
{
    public List<DetailQuestData> ActiveQuests { get; } = new();

    private QuestSummaryPanel _questSummaryPanel;
    private QuestListPanel _questListPanel;

    private TitleData _titleData;
    private List<DetailQuestData> _allQuests;
    
    protected override void Awake()
    {
        base.Awake();

        _questSummaryPanel = App.GetManager<UIManager>().GetPanel<QuestSummaryPanel>();
        _questListPanel = App.GetManager<UIManager>().GetPanel<QuestListPanel>();

        _titleData = App.GetData<TitleData>();
        _allQuests =
            _titleData.DetailQuest.Values.Where(x =>
                x.Type is QuestType.Constellation or QuestType.Side or QuestType.Wish).ToList();
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

    [ContextMenu("Add Quest")]
    public void AddQuest()
    {
        if (_allQuests.Count == 0)
        {
            Debug.LogError("No quest data found");
            return;
        }
        
        var selected = _allQuests[Random.Range(0, _allQuests.Count)];
        AddQuest(selected);

        _allQuests.Remove(selected);
    }
}
