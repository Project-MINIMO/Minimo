using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    None,
    ProdTime_Second,
    ProdTime_Percent,
    ProdAmount,
    ProdEXP,
    QuestEXP,
    SellValue,
    TimeSkipCost,
    MissionTime
}

public enum AbilityScope
{
    None,
    AllExceptFirst,
    All,
    Individual
}

public interface IMinimoAbility
{
    public AbilityScope Scope { get; }
    public float Value { get; }
    public void Apply(ProduceAdvanced building);
    public bool IsApplicableTo(ProduceAdvanced building);
    public void CalculateAbility(int level);
}

public class TimeReductionAbility : IMinimoAbility
{
    public AbilityScope Scope { get; }
    public float Value { get; private set; }

    private readonly float _baseValue;
    private readonly float _step;
    private readonly float _potential;

    public TimeReductionAbility(int id, float potential)
    {
        var titleData = App.GetData<TitleData>();
        
        var statData = titleData.UMStat[id];
        var growthData = titleData.UMStatGrowth[id];

        Scope = (AbilityScope)statData.Application;
        
        _baseValue = growthData.BaseValue;
        _step = growthData.Step;
        _potential = potential;
    }

    public void Apply(ProduceAdvanced building)
    {
        if (Scope == AbilityScope.Individual)
        {
            //building.ApplyTimeModifier(Value);
        }
    }

    public bool IsApplicableTo(ProduceAdvanced building)
    {
        return Scope == AbilityScope.Individual && building != null;
    }
    
    public void CalculateAbility(int level)
    {
        Value = Mathf.Round((_baseValue + level * _step) * _potential * 100f) / 100f;
    }
}