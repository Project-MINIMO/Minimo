using UnityEngine;

public class UserGold : IQuestRewardTarget
{
    public Sprite Icon { get; }
    public int Count { get; private set; } = 500;

    public UserGold(Sprite icon)
    {
        Icon = icon;
    }
    
    public void AddCount(int amount)
    {
        Count += amount;
        Count = Mathf.Clamp(Count, 0, int.MaxValue);
    }
}
