public class MinimoDragState : StateBase<Minimo>
{
    public MinimoDragState(Minimo owner) : base(owner) { }

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
