public class MinimoWorkState : State<MinimoObject>
{
    private readonly BehaviorTree _workTree;

    public MinimoWorkState(MinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner);
        
        var workSequence = new SequenceNode
        (
            blackboard,
            new FindWorkPositionAction(blackboard),
            new MoveAction(blackboard),
            new RepeatUntilFailNode(blackboard, new SequenceNode
                (
                    blackboard,
                    new ActiveProductionAction(blackboard),
                    new CompleteProduceAction(blackboard)
                ))
        );
        
        _workTree = new BehaviorTree(workSequence);
    }

    public override void Enter()
    {
        _workTree.Reset();
    }

    public override void Execute()
    {
        _workTree.Tick();
    }

    public override void Exit()
    {

    }
}
