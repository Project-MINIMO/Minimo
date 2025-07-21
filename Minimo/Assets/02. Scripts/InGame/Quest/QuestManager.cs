using System;
using System.Linq;
using System.Collections.Generic;

using UniRx;

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
    
    protected override void Awake()
    {
        base.Awake();
        
        GroupedQuest = App.GetData<TitleData>().Quest
            .Values
            .Where(data => data.Group != null)
            .GroupBy(data => data.Group.ID)
            .ToDictionary(data => data.Key, data => data.ToList());
    }
    
    public void AddQuest(Quest quest)
    {
        ActiveQuests.Add(quest);
    }

    public void RemoveQuest(Quest quest)
    {
        ActiveQuests.Remove(quest);
    }
    
    public void SelectQuest(Quest quest)
    {
        CurrentQuest.Value = quest;
    }

    public void DeselectQuest()
    {
        CurrentQuest.Value = null;
    }
    
    public void SubmitQuest() => SubmitQuestInternal(quest => 1);
    public void SubmitQuest(int index) => SubmitQuestInternal(quest => quest.Clear[index].Result ? 2 : 1);
    public void SubmitQuest(Item item) => SubmitQuestInternal(quest => ComputeBonus(quest, item));

    private int ComputeBonus(Quest quest, Item item)
    {
        foreach (var clear in CurrentQuest.Value.Clear)
        {
            if (clear.Target == item)
            {
                return clear.Result ? 2 : 1;
            }
        }
            
        return 0;
    }
        
    private void SubmitQuestInternal(Func<Quest, int> getBonus)
    {
        var quest = CurrentQuest.Value;
        if (quest == null) return;
    
        var bonus = getBonus(quest);
        foreach (var reward in quest.Reward)
        {
            reward.GetReward(bonus);
        }
        
        RemoveQuest(CurrentQuest.Value);
        DeselectQuest();
    }
}
