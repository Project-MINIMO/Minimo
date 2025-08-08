using System;
using UnityEngine;

public class TutorialInteractable : InteractObject
{
    public event Action onClick;

    public override void OnLongPress() { }

    public override void OnClickUp()
    {
        onClick?.Invoke();
    }
}
