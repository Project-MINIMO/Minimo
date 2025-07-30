using System;

using UnityEngine;

public class MinimoObject : MonoBehaviour
{
    public Minimo Data { get; private set; }
    public MinimoState CurrentState { get; private set; } = MinimoState.None;
    
    public event Action<MinimoObject> OnAcquired;

    private MinimoFSM _fsm;
    private ProduceAdvanced _assignedBuilding;
    
    public void Initialize(Minimo minimo)
    {
        Data = minimo;
        minimo.SetAgent(this);
        Data.OnAssignmentChanged += OnAssignmentChanged;
    }

    private void Start()
    {
        _fsm = new MinimoFSM(this);
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
