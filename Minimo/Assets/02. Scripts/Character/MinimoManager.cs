using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class MinimoManager : ManagerBase
{
    public ReactiveProperty<float> GlobalTimeReduction { get; } = new(0f);
    private float _globalTimeReduction = 0;
    
    public ReactiveProperty<float> GlobalTimeRatio { get; } = new(1f);
    private float _globalTimeRatio = 1f;
    
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
            if (a.Scope is AbilityScope.All or AbilityScope.AllExceptFirst)
            {
                switch (a.Type)
                {
                    case AbilityType.ProdTime_Second:
                        _globalTimeReduction += a.Value;
                        break;
                    
                    case AbilityType.ProdTime_Percent:
                        _globalTimeRatio *= 1 - a.Value / 100;
                        break;
                }
            }
        }

        GlobalTimeReduction.Value = _globalTimeReduction;
        GlobalTimeRatio.Value = _globalTimeRatio;
    }

    public void OnMinimoUnassigned(Minimo minimo)
    {
        foreach (var a in minimo.Abilities)
        {
            if (a.Scope is AbilityScope.All or AbilityScope.AllExceptFirst)
            {
                switch (a.Type)
                {
                    case AbilityType.ProdTime_Second:
                        _globalTimeReduction -= a.Value;
                        break;
                    
                    case AbilityType.ProdTime_Percent:
                        _globalTimeRatio /= 1 - a.Value / 100;
                        break;
                }
            }
        }
        
        GlobalTimeReduction.Value = _globalTimeReduction;
        GlobalTimeRatio.Value = _globalTimeRatio;
    }
}
