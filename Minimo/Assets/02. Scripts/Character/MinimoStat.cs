using UnityEngine;



public class MinimoStat
{
    public int ID { get; private set; }
    public AbilityType Type { get; private set; }
    public AbilityScope Scope { get; private set; }
    public float Value { get; private set; }
    
    private readonly float _baseValue;
    private readonly float _step;
    private readonly float _potential;
    
    public MinimoStat(int id, float potential)
    {
        var titleData = App.GetData<TitleData>();
        
        var statData = titleData.UMStat[id];
        var growthData = titleData.UMStatGrowth[id];
        
        ID = id;
        Type = (AbilityType)statData.StatType;
        Scope = (AbilityScope)statData.Application;
        
        _baseValue = growthData.BaseValue;
        _step = growthData.Step;
        _potential = potential;
    }
    
    public void CalculateAbility(int level)
    {
        Value = Mathf.Round((_baseValue + level * _step) * _potential * 100f) / 100f;
    }
}