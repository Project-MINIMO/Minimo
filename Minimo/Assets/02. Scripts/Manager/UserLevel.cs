using System;

using UnityEngine;

public class UserLevel : IQuestClearTarget, IQuestRewardTarget
{
    public event Action<int> OnLevelUp;
    
    public string Name { get; }
    public Sprite Icon { get; }
    
    public int Count => _experience / 100;
    
    private int _experience = 100;

    public UserLevel(Sprite icon)
    {
        Icon = icon;
        
        _experience = 100;
    }

    public void AddCount(int amount)
    {
        var prevCount = Count;
        _experience += amount;
        var newCount = Count;

        if (prevCount != newCount)
        {
            OnLevelUp?.Invoke(newCount);
        }
    }
}
