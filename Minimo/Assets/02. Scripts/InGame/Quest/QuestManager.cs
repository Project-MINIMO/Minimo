using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public Dictionary<int, List<Quest>> GroupedQuest { get; private set; } = new();
    public List<Quest> ActiveQuests { get; } = new();
    public Quest CurrentQuest;
    
    private TitleData _titleData;
    
    public event Action<List<Quest>> OnQuestsUpdated;
    
    private List<Quest> _quests;
    
    protected override void Awake()
    {
        base.Awake();
        
        _titleData = App.GetData<TitleData>();
        GroupedQuest = _titleData.Quest
            .Values
            .Where(data => data.Group != null)
            .GroupBy(data => data.Group.ID)
            .ToDictionary(data => data.Key, data => data.ToList());
        _quests = _titleData.Quest
            .Values
            .Where(quest => quest.PreQuestID == -1)
            .ToList();
    }
    
    public void AddQuest(Quest quest)
    {
        ActiveQuests.Add(quest);
        OnQuestsUpdated?.Invoke(ActiveQuests);
    }

    public void RemoveQuest(Quest quest)
    {
        ActiveQuests.Remove(quest);
        OnQuestsUpdated?.Invoke(ActiveQuests);
    }

    public void SubmitQuest()
    {
        
    }
    
    public void SubmitQuest(Item item)
    {
        
    }

    [ContextMenu("Add Quest")]
    public void AddQuest()
    {
        var selected = _quests
            .OrderBy(_ => Random.value)
            .First();
        AddQuest(selected);
        _quests.Remove(selected);
    }
}
