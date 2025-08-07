using System;
using UnityEngine;

public class TutorialInteractable : MonoBehaviour
{
    public event Action onClick;
    
    private void OnMouseDown()
    {
        if (!enabled || !gameObject.activeInHierarchy) return;

        onClick?.Invoke();
    }
}
