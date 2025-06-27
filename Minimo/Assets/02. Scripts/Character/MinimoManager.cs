using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinimoManager : ManagerBase
{
    public List<Minimo> Minimos { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Minimos = GetComponentsInChildren<Minimo>().ToList();
    }
}
