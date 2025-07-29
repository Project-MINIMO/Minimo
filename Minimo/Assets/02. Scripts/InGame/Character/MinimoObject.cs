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
        Data.OnAssignmentChanged += OnAssignmentChanged;
        AccountInfo.Instance.OnAutoAssign += OnAutoAssign;
    }

    private void Start()
    {
        FSM = new MinimoFSM(this);
        OnAssignmentChanged(Data.AssignedBuilding);
        OnAutoAssign(AccountInfo.Instance.IsAutoAssign);
    }

    private void Update()
    {
        FSM.Update();
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
            OnAutoAssign(AccountInfo.Instance.IsAutoAssign);
        }
        else
        {
            _assignedBuilding = building;
            _assignedBuilding.OnProduceStateChanged += OnProduceStateChanged;
            OnProduceStateChanged(_assignedBuilding.CurrentState);
        }
    }

    private void OnProduceStateChanged(ProduceState state)
    {
        if ((int)_currentState == (int)state) return;
        _currentState = (MinimoState)state;
        
        switch (state)
        {
            case ProduceState.Idle:
                FSM.ChangeState(MinimoState.Idle);
                break;
            
            case ProduceState.Produce:
                FSM.ChangeState(MinimoState.Work);
                break;
        }
    }

    private void OnAutoAssign(bool isAutoAssign)
    {
        if (isAutoAssign)
        {
            if (FSM.CurrentState == MinimoState.Hide)
            {
                _currentState = MinimoState.Idle;
                FSM.ChangeState(MinimoState.Idle);
            }
        }
        else
        {
            if (_assignedBuilding == null)
            {
                _currentState = MinimoState.Hide;
                FSM.ChangeState(MinimoState.Hide);
            }
            else
            {
                _currentState = MinimoState.Idle;
                FSM.ChangeState(MinimoState.Idle);
            }
        }
    }
}
