using UnityEngine;

public class MinimoHideState : State<MinimoObject>
{
    private static readonly int Default = Animator.StringToHash("Default");
    
    private MinimoObject _minimo;

    public MinimoHideState(MinimoObject owner) : base(owner)
    {
        _minimo = owner;
    }

    public override void Enter()
    {
        Animator.SetTrigger(Default);
        _minimo.transform.localPosition = Vector3.zero;
        _minimo.gameObject.SetActive(false);
    }

    public override void Execute() { }

    public override void Exit()
    {
        Animator.SetTrigger(Default);
        _minimo.transform.localPosition = Vector3.zero;
        _minimo.gameObject.SetActive(true);
    }
}
