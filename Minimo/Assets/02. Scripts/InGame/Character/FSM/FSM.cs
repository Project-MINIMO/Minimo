using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T, TStateType> where T : MonoBehaviour where TStateType : Enum
{
    private readonly Dictionary<TStateType, State<T>> _stateDictionary = new();
    private State<T> _currentState;

    protected void AddState(TStateType stateType, State<T> state)
    {
        _stateDictionary[stateType] = state;
    }

    public void ChangeState(TStateType newStateType)
    {
        _currentState?.Exit();

        if (_stateDictionary.TryGetValue(newStateType, out var newState))
        {
            _currentState = newState;
            _currentState.Enter();
        }
        else
        {
            Debug.LogWarning($"State {newStateType} not found in FSM.");
        }
    }

    public void Update()
    {
        _currentState?.Execute();
    }
}
