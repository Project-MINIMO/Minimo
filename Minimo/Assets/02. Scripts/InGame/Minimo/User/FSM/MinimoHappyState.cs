using UnityEngine;

public class MinimoHappyState : State<MinimoObject>
{
    private readonly int _isHappy = Animator.StringToHash("IsHappy");
  
    public MinimoHappyState(MinimoObject owner) : base(owner) { }

    public override void Enter()
    {
        Animator.SetBool(_isHappy, true);
    }

    public override void Execute() { }

    public override void Exit()
    {
        Animator.SetBool(_isHappy, false);
    }
}
