using UnityEngine;

public abstract class State<T> where T : MonoBehaviour
{
    protected readonly T _owner;

    public State(T owner)
    {
        _owner = owner;
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}
