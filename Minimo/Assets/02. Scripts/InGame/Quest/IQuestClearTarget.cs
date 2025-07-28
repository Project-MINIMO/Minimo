using UnityEngine;

public interface IQuestClearTarget
{
    string Name { get; }
    int Count { get; }
    Sprite Icon { get; }
    
    void AddCount(int amount);
}
