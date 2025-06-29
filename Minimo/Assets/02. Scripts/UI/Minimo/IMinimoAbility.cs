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
    public AbilityType Type { get; }
    public AbilityScope Scope { get; }
    public float Value { get; }
    public void Apply(ProduceAdvanced building);
    public bool IsApplicableTo(ProduceAdvanced building);
    public void CalculateAbility(int level);
}

public abstract class MinimoAbilityBase : IMinimoAbility
{
    public AbilityType Type { get; }
    public AbilityScope Scope { get; }
    public float Value { get; private set; }

    private readonly float _baseValue;
    private readonly float _step;
    private readonly float _potential;

    public MinimoAbilityBase(int id, float potential)
    {
        var titleData = App.GetData<TitleData>();
        
        var statData = titleData.UMStat[id];
        var growthData = titleData.UMStatGrowth[id];

        Type = (AbilityType)statData.StatType;
        Scope = (AbilityScope)statData.Application;
        
        _baseValue = growthData.BaseValue;
        _step = growthData.Step;
        _potential = potential;
    }

    public abstract void Apply(ProduceAdvanced building);

    public bool IsApplicableTo(ProduceAdvanced building)
    {
        return Scope == AbilityScope.Individual && building != null;
    }
    
    public void CalculateAbility(int level)
    {
        Value = Mathf.Round((_baseValue + level * _step) * _potential * 100f) / 100f;
    }
}

public class TimeReductionSecondAbility : MinimoAbilityBase
{
    public TimeReductionSecondAbility(int id, float potential) : base(id, potential) { }

    public override void Apply(ProduceAdvanced building)
    {
        if (Scope == AbilityScope.Individual)
        {
            building.ApplyTimeReduction(Value);
        }
    }
}

public class TimeReductionPercentAbility : MinimoAbilityBase
{
    public TimeReductionPercentAbility(int id, float potential) : base(id, potential) { }

    public override void Apply(ProduceAdvanced building)
    {
        if (Scope == AbilityScope.Individual)
        {
            building.ApplyTimeRatio(Value);
        }
    }
}