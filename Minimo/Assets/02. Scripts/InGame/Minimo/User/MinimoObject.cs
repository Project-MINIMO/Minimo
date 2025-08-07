using System;

using UnityEngine;

public class MinimoObject : MonoBehaviour
{
    public Minimo Data { get; private set; }
    public MinimoState CurrentState { get; private set; } = MinimoState.None;
    public bool IsClicked;
    public int MinimoIndex { get; private set; }
    
    public Sprite IndicatorSprite { get; private set; }
    
    public event Action<MinimoObject> OnAcquired;
    public event Action<MinimoObject> OnExpired;

    private MinimoFSM _fsm;
    private ProduceAdvanced _assignedBuilding;
    
    private void Awake()
    {
        _fsm = new MinimoFSM(this);
        IndicatorSprite = GetComponent<SpriteRenderer>().sprite;
    }
    
    public void Initialize(Minimo minimo, int index)
    {
        Data = minimo;
        minimo.SetAgent(this);
        Data.OnAssignmentChanged += OnAssignmentChanged;
        
        MinimoIndex = index;
        ApplyState(MinimoState.Swim);
    }
    
    private void Update()
    {
        _fsm.Update();
    }
    
    public void Acquire()
    {
        OnAcquired?.Invoke(this);
        EvaluateAndApplyState();
    }

    public void Expire()
    {
        OnExpired?.Invoke(this);
        CurrentState = MinimoState.None;
        _fsm.ChangeState(MinimoState.None);
    }
    
    private void OnAssignmentChanged(ProduceAdvanced building)
    {
        if (building == null)
        {
            if (_assignedBuilding != null)
            {
                _assignedBuilding.OnProduceStateChanged -= OnProduceStateChanged;
                _assignedBuilding = null;
            }
        }
        else
        {
            _assignedBuilding = building;
            _assignedBuilding.OnProduceStateChanged += OnProduceStateChanged;
        }

        EvaluateAndApplyState();
    }

    private void OnProduceStateChanged(ProduceState state) => EvaluateAndApplyState();
    
    private void EvaluateAndApplyState()
    {
        var target = DetermineTargetState();
        ApplyState(target);
    }
    
    public void ApplyState(MinimoState target)
    {
        if (target == CurrentState) return;

        CurrentState = target;
        _fsm.ChangeState(target);
    }
    
    private MinimoState DetermineTargetState()
    {
        if (_assignedBuilding == null) return MinimoState.Idle;
        if (_assignedBuilding.ActiveTask != null) return MinimoState.Work;
        return MinimoState.Idle;
    }
}
