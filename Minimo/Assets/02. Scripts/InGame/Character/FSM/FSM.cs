using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T, TStateType> where T : MonoBehaviour where TStateType : Enum
{
    public StateBase<T> CurrentState { get; private set; }
    private Dictionary<TStateType, StateBase<T>> _stateDictionary = new();

    protected void AddState(TStateType stateType, StateBase<T> state)
    {
        _stateDictionary[stateType] = state;
    }

    public void ChangeState(TStateType newStateType)
    {
        CurrentState?.Exit();

        if (_stateDictionary.TryGetValue(newStateType, out var newState))
        {
            CurrentState = newState;
            CurrentState.Enter();
        }
        else
        {
            Debug.LogWarning($"State {newStateType} not found in FSM.");
        }
    }

    public void Update()
    {
        CurrentState?.Execute();
    }
}
