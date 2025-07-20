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
    public List<Quest> ActiveQuests { get; } = new();
    public Quest CurrentQuest;
    
    private TitleData _titleData;
    private List<Quest> _allQuests;
    
    public event Action<List<Quest>> OnQuestsUpdated;
    
    protected override void Awake()
    {
        base.Awake();
        
        _titleData = App.GetData<TitleData>();
        _allQuests = _titleData.Quest.Values.ToList();
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
