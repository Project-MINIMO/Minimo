
public class MinimoIdleState : State<MinimoObject>
{
    private readonly BehaviorTree _idleTree;

    public MinimoIdleState(MinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner);
        
        var idleSequence = new SequenceNode
        (
            blackboard,
            new FindRestPositionAction(blackboard),
            new MoveAction(blackboard),
            new LayDownAction(blackboard)
        );
        
        _idleTree = new BehaviorTree(idleSequence);
    }

    public override void Enter()
    {
        _idleTree.Reset();
    }

    public override void Execute()
    {
        _idleTree.Tick();
    }

    public override void Exit()
    {

    }
}
