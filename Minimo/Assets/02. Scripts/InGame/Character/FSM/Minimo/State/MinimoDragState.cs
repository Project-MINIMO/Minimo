public class MinimoDragState : StateBase<MinimoObject>
{
    public MinimoDragState(MinimoObject owner) : base(owner) { }

    public override void Enter()
    {
        _owner.SetAnimation("IsDrag", true);
        _owner.SetSpriteOrder(1);
    }

    public override void Execute() { }

    public override void Exit()
    {
        _owner.SetAnimation("IsDrag", false);
        _owner.SetSpriteOrder(0);
    }
}
