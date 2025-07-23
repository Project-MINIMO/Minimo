using System;
using System.Linq;
using System.Collections.Generic;

using UniRx;
using UnityEngine;

public class MinimoManager : ManagerBase
{
    public ReactiveProperty<float> GlobalTimeReduction { get; } = new(0f);
    public ReactiveProperty<float> GlobalTimeRatio { get; } = new(1f);
    public ReactiveProperty<float> GlobalHarvestRatio { get; } = new(1f);
    public ReactiveProperty<float> GlobalExpRatio { get; } = new(1f);
    public ReactiveProperty<float> GlobalSellCostRatio { get; } = new(1f);
    public ReactiveProperty<float> GlobalTimeSkipCostRatio { get; } = new(1f);
    
    private readonly Dictionary<Minimo, Dictionary<AbilityType, float>> _contributions = new();
    
    protected override void Awake()
    {
        base.Awake();

        var titleData = App.GetData<TitleData>();
        var minimos = GetComponentsInChildren<MinimoObject>().ToList();
        foreach (var minimo in minimos)
        {
            minimo.Initialize(titleData.UserMinimo[minimo.transform.GetSiblingIndex()]);
        }
    }
    
    public void OnMinimoAssigned(Minimo minimo)
    {
        if (_contributions.ContainsKey(minimo))
            return;
        
        var contrib = CalculateContribution(minimo);
        _contributions[minimo] = contrib;
        
        minimo.OnLevelChanged += HandleMinimoLevelChanged;
        
        ApplyContribution(contrib, 1);
    }
    
    public void OnMinimoUnassigned(Minimo minimo)
    {
        if (!_contributions.TryGetValue(minimo, out var contrib))
            return;
        
        ApplyContribution(contrib, -1);
        
        minimo.OnLevelChanged -= HandleMinimoLevelChanged;
        _contributions.Remove(minimo);
    }
    
    private void HandleMinimoLevelChanged(Minimo minimo, int newLevel)
    {
        var oldContrib = _contributions[minimo];
        ApplyContribution(oldContrib, -1);
        
        var updatedContrib = CalculateContribution(minimo);
        _contributions[minimo] = updatedContrib;
        
        ApplyContribution(updatedContrib, 1);
    }
    
    private Dictionary<AbilityType, float> CalculateContribution(Minimo minimo)
    {
        return minimo.Abilities
            .Where(a => a.IsUnlocked && a.Scope != AbilityScope.Individual)
            .ToDictionary(a => a.Type, a => a.Value);
    }

    private void ApplyContribution(Dictionary<AbilityType, float> contrib, int sign)
    {
        foreach (var (type, value) in contrib)
        {
            var delta = sign * value;
            
            switch (type)
            {
                case AbilityType.ProdTime_Second:
                    GlobalTimeReduction.Value += delta;
                    break;
                
                case AbilityType.ProdTime_Percent:
                    GlobalTimeRatio.Value *= 1 - sign * (value / 100f);
                    break;
                
                case AbilityType.ProdAmount:
                    GlobalHarvestRatio.Value *= 1 - sign * (value / 100f);
                    break;
                
                case AbilityType.ProdEXP:
                    GlobalExpRatio.Value *= 1 + sign * (value / 100f);
                    break;
                
                case AbilityType.SellValue:
                    GlobalSellCostRatio.Value *= 1 + sign * (value / 100f);
                    break;
                
                case AbilityType.TimeSkipCost:
                    GlobalTimeSkipCostRatio.Value *= 1 - sign * (value / 100f);
                    break;
            }
        }
    }
}
