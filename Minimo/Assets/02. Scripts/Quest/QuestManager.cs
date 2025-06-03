using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : ManagerBase
{
    public List<QuestData> ActiveQuests { get; private set; }= new List<QuestData>();

    public void AddQuest(QuestData quest)
    {
        ActiveQuests.Add(quest);
    }

    public void RemoveQuest(QuestData quest)
    {
        
    }
}
