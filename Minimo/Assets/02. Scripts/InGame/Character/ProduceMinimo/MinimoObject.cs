using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class MinimoObject : MonoBehaviour
{
    public Minimo Data { get; private set; }
    public MinimoFSM FSM { get; private set; }

    private ProduceAdvanced _assignedBuilding;
    private MinimoState _currentState = MinimoState.None;

    public void Initialize(Minimo minimo)
    {
        Data = minimo;
        minimo.Agent = this;
        Data.OnAssignmentChanged += OnAssignmentChanged;
    }

    private void Start()
    {
        FSM = new MinimoFSM(this);
        OnAssignmentChanged(Data.AssignedBuilding);
    }

    private void Update()
    {
        FSM.Update();
    }
    
    public void EvaluateAndApplyState()
    {
        var target = DetermineTargetState();
        ApplyState(target);
    }
    
    private void ApplyState(MinimoState target)
    {
        if (target == _currentState) return;

        _currentState = target;
        FSM.ChangeState(target);
    }
    
    private MinimoState DetermineTargetState()
    {
        if (_assignedBuilding == null) return MinimoState.Idle;
        if (_assignedBuilding.ActiveTask != null) return MinimoState.Work;
        return MinimoState.Idle;
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
}
