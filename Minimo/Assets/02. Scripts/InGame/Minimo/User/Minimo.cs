using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Minimo
{
    public readonly int ID;
    public readonly int Type;
    
    public int Level { get; private set; }
    public ProduceAdvanced AssignedBuilding { get; private set; }
    public MinimoObject Agent { get; private set; }
    public DateTime AcquisitionDate { get; private set; }
    
    public event Action<Minimo, int> OnLevelChanged;
    public event Action<ProduceAdvanced> OnAssignmentChanged;
    
    public List<IMinimoAbility> Abilities { get; private set; }
    public List<string> AbilityDescriptions { get; private set; }
    
    public readonly string Name;
    public readonly string Description;

    
    public Minimo(UMData data, TitleData title)
    {
        ID = data.ID;
        Type = data.Type;
        
        Name = title.GetFormatString(data.Name, data.ID.ToString());
        Description = title.GetString(data.Name);
        
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
        
        AddLevel(1);
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
        
        OnLevelChanged?.Invoke(this, Level);
    }
    
    public void AssignTo(ProduceAdvanced building)
    {
        AssignedBuilding = building;
        OnAssignmentChanged?.Invoke(building);
    }

    public void Unassign()
    {
        AssignedBuilding = null;
        OnAssignmentChanged?.Invoke(null);
    }

    public void SetAgent(MinimoObject agent)
    {
        Agent = agent;
        Agent.OnAcquired += _ => AcquisitionDate = DateTime.Now;
    }
}
