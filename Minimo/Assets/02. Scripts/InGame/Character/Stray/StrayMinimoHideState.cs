using UnityEngine;
using DG.Tweening;

public class StrayMinimoHideState : State<StrayMinimoObject>
{
    private readonly Quaternion _startRotation = new(0f, 0f, 0f, 0f);
    private readonly Vector3 _endRotation = new(0f, 0f, 360f);
    
    public StrayMinimoHideState(StrayMinimoObject owner) : base(owner) { }

    public override void Enter()
    {
        Owner.transform.DOKill();
        Owner.transform.DOScale(Vector3.zero, 0.1f);
        Owner.transform.DORotate(_endRotation, 0.1f);
    }

    public override void Execute() { }

    public override void Exit()
    {
        Owner.transform.DOKill();
        Owner.transform.localScale = Vector3.zero;
        Owner.transform.rotation = _startRotation;
    }
}
