using UnityEngine;

public abstract class State<T> where T : MonoBehaviour
{
    protected readonly T Owner;
    protected readonly Animator Animator;
    
    public State(T owner)
    {
        Owner = owner;
        Animator = owner.GetComponentInChildren<Animator>();
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}
