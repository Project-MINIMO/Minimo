using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class MinimoManager : ManagerBase
{
    public ReactiveProperty<float> GlobalTimeModifier { get; } = new(1f);
    private float _globalTimeModifier = 1f;
    
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
            if (a.Scope == AbilityScope.All)
                _globalTimeModifier *= a.Value;
        }
        GlobalTimeModifier.Value = _globalTimeModifier;
    }

    public void OnMinimoUnassigned(Minimo minimo)
    {
        foreach (var a in minimo.Abilities)
        {
            if (a.Scope == AbilityScope.All)
                _globalTimeModifier /= a.Value;
        }
        GlobalTimeModifier.Value = _globalTimeModifier;
    }
}
