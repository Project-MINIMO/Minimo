using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProduceAdvanced : ProduceObject
{
    public Transform MinimoWorkingPosition;
    public bool IsMinimoWorking => MinimoWorkingPosition != null && MinimoWorkingPosition.childCount > 0;
    public string AnimTrigger;
}
