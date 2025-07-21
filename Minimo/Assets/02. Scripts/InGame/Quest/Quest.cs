using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class QuestGroup
{
    public readonly int ID;
    public readonly string Name;
    public readonly Sprite Icon;
        
    public QuestGroup(QuestGroupData data, Sprite icon, TitleData title)
    {
        ID = data.ID;
        Name = title.GetString($"STR_QUEST_{data.Name.ToUpper()}");
        Icon = icon;
    }
}

public class Quest
{
    [Serializable]
    public class QuestClear
    {
        public readonly ClearType Type;
        public readonly bool Result;
        public readonly IQuestClearTarget Target;
        public readonly int Amount;

        public int CurrentProgress => Target.Count;
        public bool IsCompleted => CurrentProgress >= Amount;
        
        public QuestClear(QuestClearData data, IQuestClearTarget target)
        {
            Type = (ClearType)data.Type;
            Result = data.Result;
            Target = target;
            Amount = data.Amount;
        }
    }
    
    [Serializable]
    public class QuestReward
    {
        public readonly IQuestRewardTarget Target;
        public readonly int Amount;
        
        public QuestReward(IQuestRewardTarget target, int amount)
        {
            Target = target;
            Amount = amount;
        }

        public void GetReward(int bonus)
        {
            Target.AddCount(Amount * bonus);
        }
    }
    
    public readonly int ID;
    public readonly QuestGroup Group;
    public readonly string Code;
    public readonly QuestType Type;
    public readonly int PreQuestID;
    public readonly int OpenLevel;
    public readonly string Name;
    public readonly string Description;
    public readonly string ClearDescription;
    public readonly QuestCondition Condition;
    public readonly QuestClear[] Clear;
    public readonly QuestReward[] Reward;
    
    public Quest(
        QuestGroup group, 
        QuestData data, 
        List<QuestClearData> clearData, 
        string[] descriptions,
        TitleData title)
    {
        ID = data.ID;
        Group = group;
        Code = data.Name;
        Type = (QuestType)data.Type;
        PreQuestID = data.PreQuestID;
        OpenLevel = data.OpenLevel;
        Name = title.GetString($"STR_QUEST_{data.Name.ToUpper()}");
        Description = title.GetString($"STR_QUEST_{data.Name.ToUpper()}_DESC");
        Condition = (QuestCondition)data.Condition;
        Clear = ParseClear(clearData, title);
        Reward = ParseReward(data.Reward, title);
        ClearDescription = GetClearDescription(descriptions);
    }
    
    private QuestClear[] ParseClear(List<QuestClearData> clearRaw, TitleData title)
    {
        if (clearRaw == null) return Array.Empty<QuestClear>();

        var result = new List<QuestClear>();
        foreach (var data in clearRaw)
        {
            IQuestClearTarget clearTarget = (ClearType)data.Type switch
            {
                ClearType.UserLevel => AccountInfo.Instance.Level,
                ClearType.Plant or ClearType.Harvest or ClearType.Wish => title.Item[data.Target],
                ClearType.Build => title.Building[data.Target],
                _ => null
            };

            result.Add(new QuestClear(data, clearTarget));
        }
        
        return result.ToArray();
    }
    
    private QuestReward[] ParseReward(string rewardRaw, TitleData title)
    {
        if (string.IsNullOrEmpty(rewardRaw)) return Array.Empty<QuestReward>();

        return rewardRaw.Split(',').Select(res =>
        {
            var parts = res.Split(':').Select(p => p.Trim()).ToArray();

            if (parts.Length < 3)
            {
                Debug.LogWarning($"[ParseCondition] Invalid format: {res}");
                return null;
            }
            
            IQuestRewardTarget rewardTarget = (RewardType)int.Parse(parts[0]) switch
            {  
                RewardType.Gold => AccountInfo.Instance.Gold,
                RewardType.Exp => AccountInfo.Instance.Level,
                RewardType.Item => title.Item[int.Parse(parts[1])],
                _ => null
            };

            return new QuestReward(rewardTarget, int.Parse(parts[2]));
        }).ToArray();
    }

    private string GetClearDescription(string[] descriptions)
    {
        var clearDescription = string.Empty;
        
        for (var i = 0; i < Clear.Length; i++)
        {
            var clear = Clear[i];
            
            if (i >= 1)
            {
                if (clear.Type == ClearType.Wish) continue;
                
                clearDescription += "\n";
            }
            
            switch (clear.Type)
            {
                case ClearType.UserLevel:
                    clearDescription += string.Format(descriptions[(int)clear.Type], clear.Amount);
                    break;

                case ClearType.Build:
                case ClearType.Plant:
                case ClearType.Harvest:
                    var name = clear.Target.Name;
                    clearDescription += string.Format(descriptions[(int)clear.Type], name, clear.Amount);
                    break;
                
                case ClearType.Wish:
                    clearDescription += descriptions[(int)clear.Type];
                    break;
            }
        }
        
        return clearDescription;
    }
}
