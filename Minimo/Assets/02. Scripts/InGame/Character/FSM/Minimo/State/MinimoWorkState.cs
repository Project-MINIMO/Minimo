public class MinimoWorkState : StateBase<MinimoObject>
{
    public MinimoWorkState(MinimoObject owner) : base(owner) { }

    public override void Enter()
    {
        _owner.SetAnimation("IsWork", true);
        _owner.SetSpriteOrder(1);
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
        _owner.SetAnimation("IsWork", false);
        _owner.SetSpriteOrder(0);
        
        _owner.transform.SetParent(null);
    }
}
