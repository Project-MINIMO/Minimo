using System.Linq;
using System.Collections.Generic;

using UniRx;
using UnityEngine;

public class MinimoManager : ManagerBase
{
    public ReactiveProperty<float> GlobalTimeReduction { get; } = new(0f);
    private float _globalTimeReduction = 0;
    
    public ReactiveProperty<float> GlobalTimeRatio { get; } = new(1f);
    private float _globalTimeRatio = 1f;
    
    public ReactiveProperty<float> GlobalHarvestRatio { get; } = new(1f);
    private float _globalHarvestRatio = 1f;
    
    public ReactiveProperty<float> GlobalExpRatio { get; } = new(1f);
    private float _globalExpRatio = 1f;
    
    public ReactiveProperty<float> GlobalSellCostRatio { get; } = new(1f);
    private float _globalSellCostRatio = 1f;
    
    public ReactiveProperty<float> GlobalTimeSkipCostRatio { get; } = new(1f);
    private float _globalTimeSkipCostRatio = 1f;
    
    public List<Minimo> Minimos { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();

        Minimos = GetComponentsInChildren<Minimo>().ToList();
    }
    
    public void OnMinimoAssigned(Minimo minimo)
    {
        foreach (var a in minimo.Abilities)
        {
            if (!a.IsUnlocked) continue;
            
            if (a.Scope is not AbilityScope.Individual)
            {
                switch (a.Type)
                {
                    case AbilityType.ProdTime_Second:
                        _globalTimeReduction += a.Value;
                        break;
                    
                    case AbilityType.ProdTime_Percent:
                        _globalTimeRatio *= 1 - a.Value / 100;
                        break;
                    
                    case AbilityType.ProdAmount:
                        _globalHarvestRatio *= 1 - a.Value / 100;
                        break;
                    
                    case AbilityType.ProdEXP:
                        _globalExpRatio *= 1 + a.Value / 100;
                        break;
                    
                    case AbilityType.SellValue:
                        _globalSellCostRatio *= 1 + a.Value / 100;
                        break;
                    
                    case AbilityType.TimeSkipCost:
                        _globalTimeSkipCostRatio *= 1 - a.Value / 100;
                        break;
                }
            }
        }

        GlobalTimeReduction.Value = _globalTimeReduction;
        GlobalTimeRatio.Value = _globalTimeRatio;
        GlobalHarvestRatio.Value = _globalHarvestRatio;
        GlobalExpRatio.Value = _globalExpRatio;
        GlobalSellCostRatio.Value = _globalSellCostRatio;
        GlobalTimeSkipCostRatio.Value = _globalTimeSkipCostRatio;
    }

    public void OnMinimoUnassigned(Minimo minimo)
    {
        foreach (var a in minimo.Abilities)
        {
            if (!a.IsUnlocked) continue;
            
            if (a.Scope is not AbilityScope.Individual)
            {
                switch (a.Type)
                {
                    case AbilityType.ProdTime_Second:
                        _globalTimeReduction -= a.Value;
                        break;
                    
                    case AbilityType.ProdTime_Percent:
                        _globalTimeRatio /= 1 - a.Value / 100;
                        break;
                    
                    case AbilityType.ProdAmount:
                        _globalHarvestRatio /= 1 - a.Value / 100;
                        break;
                    
                    case AbilityType.ProdEXP:
                        _globalExpRatio /= 1 + a.Value / 100;
                        break;
                    
                    case AbilityType.SellValue:
                        _globalSellCostRatio /= a.Value / 100;
                        break;
                    
                    case AbilityType.TimeSkipCost:
                        _globalTimeSkipCostRatio /= 1 - a.Value / 100;
                        break;
                }
            }
        }
        
        GlobalTimeReduction.Value = _globalTimeReduction;
        GlobalTimeRatio.Value = _globalTimeRatio;
        GlobalHarvestRatio.Value = _globalHarvestRatio;
        GlobalExpRatio.Value = _globalExpRatio;
        GlobalSellCostRatio.Value = _globalSellCostRatio;
        GlobalTimeSkipCostRatio.Value = _globalTimeSkipCostRatio;
    }
}
