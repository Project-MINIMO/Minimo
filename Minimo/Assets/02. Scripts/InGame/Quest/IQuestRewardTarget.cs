using UnityEngine;

public interface IQuestRewardTarget 
{
    Sprite Icon { get; }
    
    void AddCount(int amount);
}
