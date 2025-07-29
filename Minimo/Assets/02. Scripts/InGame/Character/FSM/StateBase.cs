using UnityEngine;

public abstract class State<T> where T : MonoBehaviour
{
    protected readonly Animator Animator;

    public State(T owner)
    {
        Animator = owner.GetComponent<Animator>();
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}
