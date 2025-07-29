using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinimoObject : MonoBehaviour
{
    public Minimo Data { get; private set; }
    public MinimoFSM FSM { get; private set; }
    
    private readonly int _default = Animator.StringToHash("Default");
    private readonly int _idle = Animator.StringToHash("Idle");
    private readonly int _work = Animator.StringToHash("Work");
    
    private readonly Dictionary<MinimoState, int> _states = new(4);

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
        
        _states.Add(MinimoState.Idle, _idle);
        _states.Add(MinimoState.Work, _work);
        _states.Add(MinimoState.Assign, _default);
        _states.Add(MinimoState.Hide, _default);
    }

    private void Start()
    {
        _editManager = App.GetManager<EditManager>();
        
        FSM = new MinimoFSM(this);
        OnAssignmentChanged(Data.AssignedBuilding);
        OnAutoAssign(AccountInfo.Instance.IsAutoAssign);
    }

    private void Update()
    {
        FSM.Update();

        if (_assignedBuilding == null
            && _currentState == MinimoState.Idle)
        {
            if (IsAnyEmptyAdvances())
            {
                EvaluateAndApplyState(MinimoState.Assign);
            }
        }
    }
    
    public void EvaluateAndApplyState()
    {
        var target = DetermineTargetState();
        EvaluateAndApplyState(target);
    }
    
    private void EvaluateAndApplyState(MinimoState target)
    {
        if (target == _currentState) return;

        _currentState = target;
        FSM.ChangeState(target);
        _animator.SetTrigger(_states[target]);
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
        Debug.Log("isAnyEmptyAdvances");
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
