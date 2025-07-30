using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class MinimoObject : MonoBehaviour
{
    public Minimo Data { get; private set; }
    public MinimoFSM FSM { get; private set; }

    private ProduceAdvanced _assignedBuilding;
    private MinimoState _currentState = MinimoState.None;
    
    private EditManager _editManager;
    private Animator _animator;

    public void Initialize(Minimo minimo)
    {
        Data = minimo;
        Data.OnAssignmentChanged += OnAssignmentChanged;
        AccountInfo.Instance.OnAutoAssign += OnAutoAssign;
        
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _editManager = App.GetManager<EditManager>();
        
        FSM = new MinimoFSM(this);
        OnAssignmentChanged(Data.AssignedBuilding);
    }

    private void Update()
    {
        FSM.Update();

        if (_assignedBuilding == null
            && _currentState == MinimoState.Idle)
        {
            if (IsAnyEmptyAdvances())
            {
                ApplyState(MinimoState.Assign);
            }
        }
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
        if (_assignedBuilding == null && !AccountInfo.Instance.IsAutoAssign) return MinimoState.Hide;
        
        if (_assignedBuilding == null) return MinimoState.Idle;
        
        if (_assignedBuilding.ActiveTask != null) return MinimoState.Work;
        
        return MinimoState.Idle;
    }

    private bool IsAnyEmptyAdvances()
    {
        return _editManager.ActiveAdvanceds.Any(x => x.AssignedMinimo == null);
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
    private void OnAutoAssign(bool isAutoAssign) => EvaluateAndApplyState();
}
