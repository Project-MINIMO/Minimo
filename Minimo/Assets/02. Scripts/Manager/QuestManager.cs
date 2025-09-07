using System;
using System.Linq;
using System.Collections.Generic;

using UniRx;
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
    public ReactiveProperty<Quest> CurrentQuest { get; } = new(null);
    public ReactiveCollection<Quest> ActiveQuests { get; } = new();

    private QuestSpawner _spawner;
    
    protected override void Awake()
    {
        base.Awake();
        
        GroupedQuest = App.GetData<TitleData>().Quest
            .Values
            .Where(data => data.Group != null)
            .GroupBy(data => data.Group.ID)
            .ToDictionary(data => data.Key, data => data.ToList());
    }

    private void Start()
    {
        _spawner = new QuestSpawner(this);
    }
    
    public void AddQuest(Quest quest)
    {
        ActiveQuests.Add(quest);
    }

    public void RemoveQuest(Quest quest)
    {
        ActiveQuests.Remove(quest);
        _spawner.UpdateCompletedQuest(quest.ID);
    }
    
    public void SelectQuest(Quest quest)
    {
        CurrentQuest.Value = quest;
    }

    public void DeselectQuest()
    {
        CurrentQuest.Value = null;
    }
    
    public void SubmitQuest()
    {
        var quest = CurrentQuest.Value;
        if (quest == null) return;
        quest.Clear[0].Clear();
        
        SubmitQuestInternal(quest, 1);
    }
    
    public void SubmitQuest(Quest quest)
    {
        if (!ActiveQuests.Contains(quest)) return;
        quest.Clear[0].Clear();
        
        SubmitQuestInternal(quest, 1);
    }

    public void SubmitQuest(int index)
    {
        var quest = CurrentQuest.Value;
        if (quest == null) return;
        quest.Clear[index].Clear();
        
        SubmitQuestInternal(quest, quest.Clear[index].Result ? 2 : 1);
    }
    public void SubmitQuest(Item item)
    {
        var quest = CurrentQuest.Value;
        if (quest == null) return;
        item.AddCount(-1);
        
        SubmitQuestInternal(quest, ComputeBonus(quest, item));
    }

    private int ComputeBonus(Quest quest, Item item)
    {
        foreach (var clear in quest.Clear)
        {
            if (clear.Target == item)
            {
                return clear.Result ? 2 : 1;
            }
        }
            
        return 0;
    }
        
    private void SubmitQuestInternal(Quest quest, int bonus)
    {
        if (quest == null) return;
        
        foreach (var reward in quest.Reward)
        {
            reward.GetReward(bonus);
        }
        
        RemoveQuest(quest);
        if (quest.Type == QuestType.Constellation)
        {
            var first = ActiveQuests
                .Where(item => item.Type == QuestType.Constellation)
                .FirstOrDefault(item => item.Group.ID == quest.Group.ID);
            if (first != null)
            {
                SelectQuest(first);
            }
        }
        else
        {
            DeselectQuest();
        }
    }
}
