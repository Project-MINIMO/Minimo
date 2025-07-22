using System;
using System.Collections.Generic;
using UnityEngine;

public class Minimo
{
    public event Action<int> OnMinimoLevelChanged;
    public event Action<ProduceAdvanced> OnAssignedBuildingChanged;
    
    public readonly int ID;
    public readonly int Type;
    public List<IMinimoAbility> Abilities { get; private set; }
    public List<string> AbilityDescriptions { get; private set; }
    public readonly string Name;
    public readonly string Description;
    public readonly int AcquisitionDate;
    public ProduceAdvanced AssignedBuilding { get; private set; }
    
    public int Level { get; private set; }
    
    public Minimo(UMData data, TitleData title)
    {
        Name = title.GetFormatString(data.Name, data.ID.ToString());
        float potentialValue = data.Potential;
        var rawPotential = 1 + (potentialValue - 1) * (((float)title.Common["PotentialGap"] - 1) / 9);
        var potential = Mathf.Round(rawPotential * 100f) / 100f;
        
        Abilities = new List<IMinimoAbility>
        {
            CreateAbility(title.UMStat[data.StatType1].StatType, data.StatType1, potential),
            CreateAbility(title.UMStat[data.StatType2].StatType, data.StatType2, potential),
            CreateAbility(title.UMStat[data.StatType3].StatType, data.StatType3, potential),
        };
        
        AbilityDescriptions = new List<string>
        {
            title.GetString(title.UMStat[data.StatType1].Name),
            title.GetString(title.UMStat[data.StatType2].Name),
            title.GetString(title.UMStat[data.StatType3].Name)
        };
    }
    
    private IMinimoAbility CreateAbility(int abilityType, int id, float potential) => (AbilityType)abilityType switch
    {
        AbilityType.None => null,
        AbilityType.ProdTime_Second => new ProduceTimeSecondAbility(id, potential),
        AbilityType.ProdTime_Percent => new ProduceTimePercentAbility(id, potential),
        AbilityType.ProdAmount => new ProduceAmountAbility(id, potential),
        AbilityType.ProdEXP => new ProduceExperienceAbility(id, potential),
        AbilityType.QuestEXP => new QuestExperienceAbility(id, potential),
        AbilityType.SellValue => new SellValueAbility(id, potential),
        AbilityType.TimeSkipCost => new TimeSkipCostAbility(id, potential),
        AbilityType.MissionTime => new MissionTimeAbility(id, potential),
        _ => null
    };
    
    public void AddLevel(int amount)
    {
        Level += amount;
        Level = Mathf.Clamp(Level, 1, 30);

        foreach (var ability in Abilities)
        {
            ability.CalculateAbility(Level);
        }
        
        OnMinimoLevelChanged?.Invoke(Level);
    }
}
