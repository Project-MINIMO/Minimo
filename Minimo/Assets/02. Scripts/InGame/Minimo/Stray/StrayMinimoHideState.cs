using UnityEngine;
using DG.Tweening;

public class StrayMinimoHideState : State<StrayMinimoObject>
{
    public StrayMinimoHideState(StrayMinimoObject owner) : base(owner) { }

    public override void Enter()
    {
        Owner.transform.DOKill();
        Owner.transform.DOScale(Vector3.zero, 0.2f);
    }

    public override void Execute() { }

    public override void Exit()
    {
        Owner.transform.DOKill();
        Owner.transform.localScale = Vector3.zero;
    }
}
