using UnityEngine;

public class StrayMinimoIdleState : State<StrayMinimoObject>
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");
    
    private readonly BehaviorTree _idleTree;

    public StrayMinimoIdleState(StrayMinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var idleSequence = new SequenceNode
        (
            blackboard,
            new FindStrayPositionAction(blackboard),
            new MoveAction(blackboard)
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
        Animator.speed = 1;
        Animator.SetBool(IsWalk, false);
    }
}
