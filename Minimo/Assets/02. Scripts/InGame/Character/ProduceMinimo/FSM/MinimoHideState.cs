using UnityEngine;

public class MinimoHideState : State<MinimoObject>
{
    private readonly int _default = Animator.StringToHash("Default");
    
    public MinimoHideState(MinimoObject owner) : base(owner) { }

    public override void Enter()
    {
        Animator.SetTrigger(_default);
        Owner.transform.localPosition = Vector3.zero;
        Owner.gameObject.SetActive(false);
    }

    public override void Execute() { }

    public override void Exit()
    {
        Owner.transform.localPosition = Vector3.zero;
        Owner.gameObject.SetActive(true);
    }
}
