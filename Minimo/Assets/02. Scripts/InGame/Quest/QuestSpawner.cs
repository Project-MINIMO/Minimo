using System.Linq;
using System.Collections.Generic;

using UniRx;
using UnityEngine;

public class QuestSpawner
{
    private readonly HashSet<int> _spawnedIds = new();
    private readonly HashSet<Quest> _waitingForLevel = new();
    private readonly Dictionary<int, List<Quest>> _dependents;
    
    private readonly QuestManager _questManager;
    private readonly UserLevel _level; 

    public QuestSpawner(QuestManager questManager)
    {
        _questManager = questManager;
        _level = AccountInfo.Instance.Level;  
        
        _dependents = App.GetData<TitleData>().Quest
            .Values
            .GroupBy(data => data.PreQuestID)
            .ToDictionary(data => data.Key, data => data.ToList());

        if (_dependents.TryGetValue(-1, out var rootQuests))
        {
            foreach (var quest in rootQuests)
            {
                _waitingForLevel.Add(quest); 
            }
        }
        
        _level.OnLevelUp += TrySpawnByLevel;
        TrySpawnByLevel(_level.Count);
    }

    public void UpdateCompletedQuest(int questId)
    {
        if (_dependents.TryGetValue(questId, out var list))
        {
            foreach (var quest in list.Where(quest => !_spawnedIds.Contains(quest.ID)))
            {
                _waitingForLevel.Add(quest);
            }
        }
        
        TrySpawnByLevel(_level.Count);
    }
    
    private void TrySpawnByLevel(int level)
    {
        var currentLevel = _level.Count;

        var filteredQuest = _waitingForLevel
            .Where(quest => quest.OpenLevel <= currentLevel)
            .ToList();

        foreach (var quest in filteredQuest)
        {
            Spawn(quest);
            _waitingForLevel.Remove(quest);
        }
    }
    
    private void Spawn(Quest quest)
    {
        _questManager.AddQuest(quest);
        _spawnedIds.Add(quest.ID);

        Debug.Log($"[QuestSpawner] Spawned Quest ID = {quest.ID}");
    }
}
