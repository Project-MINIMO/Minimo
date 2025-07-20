using System;
using System.Linq;
using System.Collections.Generic;

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
    
    protected override void Awake()
    {
        base.Awake();
        
        _titleData = App.GetData<TitleData>();
        GroupedQuest = _titleData.Quest
            .Values
            .GroupBy(data => data.Group.ID)
            .ToDictionary(data => data.Key, data => data.ToList());
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
        if (GroupedQuest.Count == 0)
        {
            Debug.LogError("No quest data found");
            return;
        }
        
        var selected = GroupedQuest
            .OrderBy(_ => Random.value)
            .First();
        AddQuest(selected.Value[0]);
    }
}
