using UnityEngine;

public class MinimoObject : MonoBehaviour
{
    public Minimo Data { get; private set; }
    public MinimoFSM FSM { get; private set; }

    private ProduceAdvanced _assignedBuilding;
    private ProduceState _currentState = ProduceState.Complete;

    public void Initialize(Minimo minimo)
    {
        Data = minimo;
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

    private void OnAssignmentChanged(ProduceAdvanced building)
    {
        if (building == null)
        {
            if (_assignedBuilding != null)
            {
                _assignedBuilding.OnProduceStateChanged -= OnProduceStateChanged;
                _assignedBuilding = null;
            }
            OnProduceStateChanged(ProduceState.Idle);
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
        if (_currentState == state) return;
        _currentState = state;
        
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
}
