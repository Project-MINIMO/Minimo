using UnityEngine;

public class MinimoIdleState : State<MinimoObject>
{
    private readonly BehaviorTree _idleTree;
    private readonly Blackboard _blackboard;

    public MinimoIdleState(MinimoObject owner) : base(owner)
    {
        _blackboard = new Blackboard(owner);
        
        var idleSequence = new SequenceNode
        (
            _blackboard,
            new FindRestPositionAction(_blackboard),
            new MoveAction(_blackboard),
            new LayDownAction(_blackboard)
        );
        
        _idleTree = new BehaviorTree(idleSequence);
    }

    public override void Enter()
    {
        Debug.Log("MinimoIdleState");
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
