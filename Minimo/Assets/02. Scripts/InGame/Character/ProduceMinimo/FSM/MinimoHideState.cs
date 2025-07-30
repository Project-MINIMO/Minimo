using UnityEngine;

public class MinimoHideState : State<MinimoObject>
{
    private MinimoObject _minimo;

    public MinimoHideState(MinimoObject owner) : base(owner)
    {
        _minimo = owner;
    }

    public override void Enter()
    {
        _minimo.transform.localPosition = Vector3.zero;
        _minimo.gameObject.SetActive(false);
    }

    public override void Execute() { }

    public override void Exit()
    {
        _minimo.transform.localPosition = Vector3.zero;
        _minimo.gameObject.SetActive(true);
    }
}
