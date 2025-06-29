using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ProduceAdvanced : ProduceObject
{
    public Transform MinimoWorkingPosition;
    public bool IsMinimoWorking => MinimoWorkingPosition != null && MinimoWorkingPosition.childCount > 0;
    public string AnimTrigger;

    private Minimo _placedMinimo;

    
    protected override void Awake()
    {
        base.Awake();

        MinimoWorkingPosition = transform.GetChild(2);
    }

    public void PlaceMinimo(Minimo minimo)
    {
        if (_placedMinimo != null)
        {
            _placedMinimo.SetChillState();
        }
        
        _placedMinimo = minimo;
    }
}
