using System;
using System.Linq;

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
        public readonly int Target;
        public readonly int Amount;
        public QuestClear(ClearType type, int target, int amount)
        {
            Type = type;
            Target = target;
            Amount = amount;
        }
    }

    [Serializable]
    public class QuestReward
    {
        public readonly RewardType Type;
        public readonly int Target;
        public readonly int Amount;
        public QuestReward(RewardType type, int target, int amount)
        {
            Type = type;
            Target = target;
            Amount = amount;
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
    
    public Quest(QuestGroup group, QuestData data, TitleData title)
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
        Clear = ParseClear(data.Clear);
        Reward = ParseReward(data.Reward);
        
        var descriptionStrings = new[]
        {
            title.GetString("STR_QUEST_CLEAR_LEVEL"),
            title.GetString("STR_QUEST_CLEAR_PREP"),
            title.GetString("STR_QUEST_CLEAR_HARVEST"),
            title.GetString("STR_QUEST_CLEAR_CRAFT"),
            title.GetString("STR_QUEST_CLEAR_WISH"),
            title.GetString("STR_QUEST_CLEAR_BUILD"),
        };
        for (var i = 0; i < Clear.Length; i++)
        {
            if (i >= 1) ClearDescription += "\n";
            
            var clear = Clear[i];
            
            switch (clear.Type)
            {
                case ClearType.Wish:
                    ClearDescription += descriptionStrings[(int)clear.Type];
                    break;
                
                case ClearType.UserLevel:
                    ClearDescription += string.Format(descriptionStrings[(int)clear.Type], clear.Amount);
                    break;
                
                case ClearType.Build:
                {
                    var target = title.Building[clear.Target];
                    var name = target.Name;
                    ClearDescription += string.Format(descriptionStrings[(int)clear.Type], title.GetString(name), clear.Amount);
                    break;
                }
                
                case ClearType.Plant:
                case ClearType.Harvest:
                case ClearType.Craft:
                {
                    //var target = title.Item[clear.Target];
                    //var name = title.GetString($"STR_ITEM_{target.Name.ToUpper()}_NAME");
                    //ClearDescription += string.Format(descriptionStrings[(int)clear.Type], title.GetString(name), clear.Amount);
                    break;
                }
            }
        }
    }
    
    private QuestClear[] ParseClear(string clearRaw)
    {
        if (string.IsNullOrEmpty(clearRaw)) return Array.Empty<QuestClear>();

        return clearRaw.Split(',').Select(res =>
        {
            var parts = res.Split(':').Select(p => p.Trim()).ToArray();

            if (parts.Length < 3)
            {
                Debug.LogWarning($"[ParseCondition] Invalid format: {res}");
                return null;
            }

            return new QuestClear(
                (ClearType)int.Parse(parts[0]), 
                int.Parse(parts[1]), 
                int.Parse(parts[2]));
        }).ToArray();
    }
    
    private QuestReward[] ParseReward(string rewardRaw)
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

            return new QuestReward(
                (RewardType)int.Parse(parts[0]), 
                int.Parse(parts[1]), 
                int.Parse(parts[2]));
        }).ToArray();
    }
}
