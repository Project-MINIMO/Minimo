public class StrayMinimoHideState : State<StrayMinimoObject>
{
    public StrayMinimoHideState(StrayMinimoObject owner) : base(owner) { }

    public override void Enter()
    {
        Owner.gameObject.SetActive(false);
    }

    public override void Execute() { }

    public override void Exit()
    {
        Owner.gameObject.SetActive(true);
    }
}
