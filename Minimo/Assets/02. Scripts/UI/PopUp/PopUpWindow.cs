using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class PopUpWindow : MonoBehaviour
{
    public event Action OnAssign;
    public abstract PopUpType Type();

    [SerializeField] protected Button AssignBtn;
    
    protected virtual void Awake()
    {
        AssignBtn.onClick.AddListener(Assign);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    
    public virtual void Show(ProduceAdvanced building, Minimo minimo) { }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Assign()
    {
        OnAssign?.Invoke();
    }
}
